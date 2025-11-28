using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Project.Core.Constants;
using Project.Core.Entities;
using Project.Core.Interfaces;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Services;

public class TwoFactorService : ITwoFactorService
{
    private readonly AppDbContext _context;
    private readonly ISettingManager _settingManager;
    private readonly ICurrentUser _currentUser;
    private readonly ICurrentTenant _currentTenant;

    // Configuration constants
    private const int OtpLength = 4;
    private const int OtpExpirationMinutes = 5;
    private const int BackupCodeCount = 10;
    private const int BackupCodeLength = 8;
    private const int TrustedDeviceExpirationDays = 30;
    private const int MaxOtpRequestsPer15Minutes = 3;
    private const int MaxOtpVerificationAttempts = 5;

    public TwoFactorService(
        AppDbContext context,
        ISettingManager settingManager,
        ICurrentUser currentUser,
        ICurrentTenant currentTenant)
    {
        _context = context;
        _settingManager = settingManager;
        _currentUser = currentUser;
        _currentTenant = currentTenant;
    }

    public async Task<string> GenerateOtpAsync(Guid userId)
    {
        // Invalidate any existing OTPs for this user
        await InvalidateOtpAsync(userId);

        // Generate a random 4-digit code
        var code = GenerateRandomNumericCode(OtpLength);

        var otpCode = new TwoFactorCode
        {
            UserId = userId,
            Code = code,
            ExpiresAt = DateTime.UtcNow.AddMinutes(OtpExpirationMinutes),
            CreatedAt = DateTime.UtcNow
        };

        _context.TwoFactorCodes.Add(otpCode);
        await _context.SaveChangesAsync();

        return code;
    }

    public async Task<bool> ValidateOtpAsync(Guid userId, string code)
    {
        var otpCode = await _context.TwoFactorCodes
            .Where(o => o.UserId == userId && !o.IsUsed && o.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();

        if (otpCode == null)
            return false;

        // Increment attempt count
        otpCode.AttemptCount++;
        await _context.SaveChangesAsync();

        if (otpCode.Code != code)
            return false;

        // Mark as used
        otpCode.IsUsed = true;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ValidateBackupCodeAsync(Guid userId, string code)
    {
        var backupCodes = await _context.TwoFactorBackupCodes
            .Where(b => b.UserId == userId && !b.IsUsed && !b.IsDeleted)
            .ToListAsync();

        foreach (var backupCode in backupCodes)
        {
            if (BCrypt.Net.BCrypt.Verify(code, backupCode.CodeHash))
            {
                // Mark as used
                backupCode.IsUsed = true;
                backupCode.UsedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return true;
            }
        }

        return false;
    }

    public async Task<List<string>> GenerateBackupCodesAsync(Guid userId)
    {
        // Soft delete all existing backup codes
        var existingCodes = await _context.TwoFactorBackupCodes
            .Where(b => b.UserId == userId && !b.IsDeleted)
            .ToListAsync();

        foreach (var code in existingCodes)
        {
            code.IsDeleted = true;
        }

        // Generate new backup codes
        var codes = new List<string>();
        for (int i = 0; i < BackupCodeCount; i++)
        {
            var code = GenerateRandomAlphanumericCode(BackupCodeLength);
            codes.Add(code);

            var backupCode = new TwoFactorBackupCode
            {
                UserId = userId,
                CodeHash = BCrypt.Net.BCrypt.HashPassword(code),
                CreatedBy = _currentUser.Id,
                CreatedOn = DateTime.UtcNow,
                TenantId = _currentTenant.Id
            };

            _context.TwoFactorBackupCodes.Add(backupCode);
        }

        await _context.SaveChangesAsync();
        return codes;
    }

    public async Task<bool> IsTwoFactorRequiredAsync(Guid userId, Guid? tenantId, string[] roles, string? deviceFingerprint = null)
    {
        // Check if device is trusted
        if (!string.IsNullOrEmpty(deviceFingerprint))
        {
            var isTrusted = await IsTrustedDeviceAsync(userId, deviceFingerprint);
            if (isTrusted)
                return false;
        }

        // Check if 2FA is mandatory for any of the user's roles
        var mandatoryRoles = await GetMandatoryRolesAsync(tenantId);
        if (mandatoryRoles.Any() && roles.Any(r => mandatoryRoles.Contains(r, StringComparer.OrdinalIgnoreCase)))
        {
            return true;
        }

        // Check settings hierarchy: User → Tenant → Global
        var userSetting = await _settingManager.GetForUserAsync(SettingNames.TwoFactorEnabled, userId);
        if (userSetting != null)
            return userSetting.Equals("true", StringComparison.OrdinalIgnoreCase);

        if (tenantId.HasValue)
        {
            var tenantSetting = await _settingManager.GetForTenantAsync(SettingNames.TwoFactorEnabled, tenantId.Value);
            if (tenantSetting != null)
                return tenantSetting.Equals("true", StringComparison.OrdinalIgnoreCase);
        }

        var globalSetting = await _settingManager.GetOrNullAsync(SettingNames.TwoFactorEnabled);
        if (globalSetting != null)
            return globalSetting.Equals("true", StringComparison.OrdinalIgnoreCase);

        // Default: false
        return false;
    }

    public async Task<bool> IsTrustedDeviceAsync(Guid userId, string deviceFingerprint)
    {
        return await _context.TrustedDevices
            .AnyAsync(d => d.UserId == userId 
                && d.DeviceFingerprint == deviceFingerprint 
                && !d.IsDeleted 
                && d.ExpiresAt > DateTime.UtcNow);
    }

    public async Task AddTrustedDeviceAsync(Guid userId, string deviceFingerprint, string deviceName)
    {
        var trustedDevice = new TrustedDevice
        {
            UserId = userId,
            DeviceFingerprint = deviceFingerprint,
            DeviceName = deviceName,
            ExpiresAt = DateTime.UtcNow.AddDays(TrustedDeviceExpirationDays),
            LastUsedAt = DateTime.UtcNow,
            CreatedBy = _currentUser.Id,
            CreatedOn = DateTime.UtcNow,
            TenantId = _currentTenant.Id
        };

        _context.TrustedDevices.Add(trustedDevice);
        await _context.SaveChangesAsync();
    }

    public async Task<List<TrustedDevice>> GetTrustedDevicesAsync(Guid userId)
    {
        return await _context.TrustedDevices
            .Where(d => d.UserId == userId && !d.IsDeleted)
            .OrderByDescending(d => d.CreatedOn)
            .ToListAsync();
    }

    public async Task RevokeTrustedDeviceAsync(Guid userId, Guid deviceId)
    {
        var device = await _context.TrustedDevices
            .FirstOrDefaultAsync(d => d.Id == deviceId && d.UserId == userId && !d.IsDeleted);

        if (device != null)
        {
            device.IsDeleted = true;
            device.LastModifiedBy = _currentUser.Id;
            device.LastModifiedOn = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> CanRequestOtpAsync(Guid userId)
    {
        var cutoffTime = DateTime.UtcNow.AddMinutes(-15);
        var recentRequests = await _context.TwoFactorCodes
            .Where(o => o.UserId == userId && o.CreatedAt > cutoffTime)
            .CountAsync();

        return recentRequests < MaxOtpRequestsPer15Minutes;
    }

    public async Task<bool> CanVerifyOtpAsync(Guid userId)
    {
        var otpCode = await _context.TwoFactorCodes
            .Where(o => o.UserId == userId && !o.IsUsed && o.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();

        if (otpCode == null)
            return true; // No active OTP, so verification is allowed

        return otpCode.AttemptCount < MaxOtpVerificationAttempts;
    }

    public async Task InvalidateOtpAsync(Guid userId)
    {
        var existingCodes = await _context.TwoFactorCodes
            .Where(o => o.UserId == userId && !o.IsUsed)
            .ToListAsync();

        foreach (var code in existingCodes)
        {
            code.IsUsed = true;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<int> GetRemainingBackupCodesCountAsync(Guid userId)
    {
        return await _context.TwoFactorBackupCodes
            .Where(b => b.UserId == userId && !b.IsUsed && !b.IsDeleted)
            .CountAsync();
    }

    // Helper methods

    private string GenerateRandomNumericCode(int length)
    {
        var random = new Random();
        var code = "";
        for (int i = 0; i < length; i++)
        {
            code += random.Next(0, 10).ToString();
        }
        return code;
    }

    private string GenerateRandomAlphanumericCode(int length)
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // Exclude ambiguous characters
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private async Task<List<string>> GetMandatoryRolesAsync(Guid? tenantId)
    {
        string? rolesString = null;

        if (tenantId.HasValue)
        {
            rolesString = await _settingManager.GetForTenantAsync(SettingNames.TwoFactorMandatoryForRoles, tenantId.Value);
        }

        if (rolesString == null)
        {
            rolesString = await _settingManager.GetOrNullAsync(SettingNames.TwoFactorMandatoryForRoles);
        }

        if (string.IsNullOrWhiteSpace(rolesString))
            return new List<string>();

        return rolesString.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(r => r.Trim())
            .ToList();
    }

    // SMS OTP Methods
    public async Task<string> GenerateSmsOtpAsync(Guid userId)
    {
        // Get user to verify phone number
        var user = await _context.Users.FindAsync(userId);
        if (user == null || string.IsNullOrEmpty(user.PhoneNumber))
            throw new InvalidOperationException("User phone number is not set");

        if (!user.IsPhoneNumberConfirmed)
            throw new InvalidOperationException("Phone number must be verified before using SMS 2FA");

        // Invalidate any existing OTPs for this user
        await InvalidateOtpAsync(userId);

        // Generate a random 4-digit code
        var code = GenerateRandomNumericCode(OtpLength);

        var otpCode = new TwoFactorCode
        {
            UserId = userId,
            Code = code,
            Provider = "Sms",
            ExpiresAt = DateTime.UtcNow.AddMinutes(OtpExpirationMinutes),
            CreatedAt = DateTime.UtcNow
        };

        _context.TwoFactorCodes.Add(otpCode);
        await _context.SaveChangesAsync();

        return code;
    }

    public async Task<bool> ValidateSmsOtpAsync(Guid userId, string code)
    {
        var otpCode = await _context.TwoFactorCodes
            .Where(o => o.UserId == userId && o.Provider == "Sms" && !o.IsUsed && o.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();

        if (otpCode == null)
            return false;

        // Increment attempt count
        otpCode.AttemptCount++;
        await _context.SaveChangesAsync();

        if (otpCode.Code != code)
            return false;

        // Mark as used
        otpCode.IsUsed = true;
        await _context.SaveChangesAsync();

        return true;
    }

    // Authenticator (TOTP) Methods
    public async Task<(string Secret, string QrCodeUri)> GenerateAuthenticatorSecretAsync(Guid userId, string issuer, string accountName)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new InvalidOperationException("User not found");

        // Generate new secret
        var secret = Project.Infrastructure.Security.TotpHelper.GenerateSecret();
        
        // Store encrypted secret temporarily (user must verify before it's enabled)
        user.AuthenticatorKey = secret; // TODO: Encrypt this in production
        await _context.SaveChangesAsync();

        // Generate QR code URI
        var qrCodeUri = Project.Infrastructure.Security.TotpHelper.GenerateQrCodeUri(secret, issuer, accountName);

        return (secret, qrCodeUri);
    }

    public async Task<bool> EnableAuthenticatorAsync(Guid userId, string verificationCode)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null || string.IsNullOrEmpty(user.AuthenticatorKey))
            return false;

        // Validate the verification code
        var isValid = Project.Infrastructure.Security.TotpHelper.ValidateCode(user.AuthenticatorKey, verificationCode);
        
        if (!isValid)
            return false;

        // Enable authenticator
        user.IsAuthenticatorEnabled = true;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task DisableAuthenticatorAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return;

        user.IsAuthenticatorEnabled = false;
        user.AuthenticatorKey = null;
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ValidateTotpAsync(Guid userId, string code)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null || !user.IsAuthenticatorEnabled || string.IsNullOrEmpty(user.AuthenticatorKey))
            return false;

        return Project.Infrastructure.Security.TotpHelper.ValidateCode(user.AuthenticatorKey, code);
    }

    // Phone Verification Methods
    public async Task SendPhoneVerificationCodeAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null || string.IsNullOrEmpty(user.PhoneNumber))
            throw new InvalidOperationException("User phone number is not set");

        // Generate a 6-digit verification code
        var code = GenerateRandomNumericCode(6);

        var otpCode = new TwoFactorCode
        {
            UserId = userId,
            Code = code,
            Provider = "PhoneVerification",
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            CreatedAt = DateTime.UtcNow
        };

        _context.TwoFactorCodes.Add(otpCode);
        await _context.SaveChangesAsync();

        // SMS will be sent by the caller (TwoFactorAppService)
    }

    public async Task<bool> VerifyPhoneNumberAsync(Guid userId, string code)
    {
        var otpCode = await _context.TwoFactorCodes
            .Where(o => o.UserId == userId && o.Provider == "PhoneVerification" && !o.IsUsed && o.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();

        if (otpCode == null || otpCode.Code != code)
            return false;

        // Mark as used
        otpCode.IsUsed = true;

        // Update user's phone confirmation status
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.IsPhoneNumberConfirmed = true;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    // Provider Management
    public async Task<string> GetPreferredProviderAsync(Guid userId)
    {
        var setting = await _settingManager.GetForUserAsync(SettingNames.PreferredTwoFactorProvider, userId);
        return setting ?? "Email"; // Default to Email
    }

    public async Task SetPreferredProviderAsync(Guid userId, string provider)
    {
        // Validate provider
        if (provider != "Email" && provider != "Sms" && provider != "Authenticator")
            throw new ArgumentException("Invalid provider. Must be 'Email', 'Sms', or 'Authenticator'");

        // Validate prerequisites
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new InvalidOperationException("User not found");

        if (provider == "Sms" && !user.IsPhoneNumberConfirmed)
            throw new InvalidOperationException("Phone number must be verified before using SMS 2FA");

        if (provider == "Authenticator" && !user.IsAuthenticatorEnabled)
            throw new InvalidOperationException("Authenticator must be set up before using Authenticator 2FA");

        await _settingManager.SetForUserAsync(SettingNames.PreferredTwoFactorProvider, provider, userId);
    }
}
