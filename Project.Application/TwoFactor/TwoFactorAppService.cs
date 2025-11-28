using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project.Application.Common.Dtos;
using Project.Application.TwoFactor.Dtos;
using Project.Core.Constants;
using Project.Core.Emailing;
using Project.Core.Interfaces;
using Project.Infrastructure.Emailing.Templates;

namespace Project.Application.TwoFactor;

/// <summary>
/// Application service for managing two-factor authentication.
/// </summary>
public class TwoFactorAppService
{
    private readonly ITwoFactorService _twoFactorService;
    private readonly ISettingManager _settingManager;
    private readonly ICurrentUser _currentUser;
    private readonly ICurrentTenant _currentTenant;
    private readonly IEmailSender _emailSender;

    public TwoFactorAppService(
        ITwoFactorService twoFactorService,
        ISettingManager settingManager,
        ICurrentUser currentUser,
        ICurrentTenant currentTenant,
        IEmailSender emailSender)
    {
        _twoFactorService = twoFactorService;
        _settingManager = settingManager;
        _currentUser = currentUser;
        _currentTenant = currentTenant;
        _emailSender = emailSender;
    }

    public async Task<TwoFactorStatusDto> GetStatusAsync()
    {
        if (!_currentUser.Id.HasValue)
            throw new UnauthorizedAccessException("User not authenticated");

        var userId = _currentUser.Id.Value;
        var tenantId = _currentTenant.Id;

        // Check if 2FA is enabled for the user
        var userSetting = await _settingManager.GetForUserAsync(SettingNames.TwoFactorEnabled, userId);
        var isEnabled = userSetting?.Equals("true", StringComparison.OrdinalIgnoreCase) ?? false;

        // Check if 2FA is mandatory
        var isMandatory = await IsMandatoryForCurrentUserAsync();

        // Get backup codes count
        var backupCodesRemaining = await _twoFactorService.GetRemainingBackupCodesCountAsync(userId);

        // Get trusted devices count
        var trustedDevices = await _twoFactorService.GetTrustedDevicesAsync(userId);

        return new TwoFactorStatusDto
        {
            IsEnabled = isEnabled || isMandatory,
            IsMandatory = isMandatory,
            BackupCodesRemaining = backupCodesRemaining,
            TrustedDevicesCount = trustedDevices.Count
        };
    }

    public async Task<BackupCodesDto> EnableTwoFactorAsync()
    {
        if (!_currentUser.Id.HasValue)
            throw new UnauthorizedAccessException("User not authenticated");

        var userId = _currentUser.Id.Value;

        // Enable 2FA for the user
        await _settingManager.SetForUserAsync(SettingNames.TwoFactorEnabled, "true", userId);

        // Generate backup codes
        var codes = await _twoFactorService.GenerateBackupCodesAsync(userId);

        // Send backup codes via email
        if (!string.IsNullOrEmpty(_currentUser.Email))
        {
            var emailBody = TwoFactorEmailTemplate.GenerateBackupCodesEmail(codes);
            await _emailSender.QueueAsync(_currentUser.Email, "Your Two-Factor Backup Codes", emailBody, isBodyHtml: true);
        }

        return new BackupCodesDto { Codes = codes };
    }

    public async Task DisableTwoFactorAsync()
    {
        if (!_currentUser.Id.HasValue)
            throw new UnauthorizedAccessException("User not authenticated");

        // Check if 2FA is mandatory for the user
        var isMandatory = await IsMandatoryForCurrentUserAsync();
        if (isMandatory)
            throw new InvalidOperationException("Two-factor authentication is mandatory for your role and cannot be disabled");

        var userId = _currentUser.Id.Value;

        // Disable 2FA for the user
        await _settingManager.DeleteForUserAsync(SettingNames.TwoFactorEnabled, userId);
    }

    public async Task<BackupCodesDto> RegenerateBackupCodesAsync()
    {
        if (!_currentUser.Id.HasValue)
            throw new UnauthorizedAccessException("User not authenticated");

        var userId = _currentUser.Id.Value;

        // Generate new backup codes
        var codes = await _twoFactorService.GenerateBackupCodesAsync(userId);

        // Send backup codes via email
        if (!string.IsNullOrEmpty(_currentUser.Email))
        {
            var emailBody = TwoFactorEmailTemplate.GenerateBackupCodesEmail(codes);
            await _emailSender.QueueAsync(_currentUser.Email, "Your New Two-Factor Backup Codes", emailBody, isBodyHtml: true);
        }

        return new BackupCodesDto { Codes = codes };
    }

    public async Task<List<TrustedDeviceDto>> GetTrustedDevicesAsync()
    {
        if (!_currentUser.Id.HasValue)
            throw new UnauthorizedAccessException("User not authenticated");

        var userId = _currentUser.Id.Value;
        var devices = await _twoFactorService.GetTrustedDevicesAsync(userId);

        return devices.Select(d => new TrustedDeviceDto
        {
            Id = d.Id,
            DeviceName = d.DeviceName,
            CreatedOn = d.CreatedOn,
            LastUsedAt = d.LastUsedAt,
            ExpiresAt = d.ExpiresAt
        }).ToList();
    }

    public async Task RevokeTrustedDeviceAsync(Guid deviceId)
    {
        if (!_currentUser.Id.HasValue)
            throw new UnauthorizedAccessException("User not authenticated");

        var userId = _currentUser.Id.Value;
        await _twoFactorService.RevokeTrustedDeviceAsync(userId, deviceId);
    }

    private async Task<bool> IsMandatoryForCurrentUserAsync()
    {

        var mandatoryRolesString = _currentTenant.Id.HasValue
            ? await _settingManager.GetForTenantAsync(SettingNames.TwoFactorMandatoryForRoles, _currentTenant.Id.Value)
            : await _settingManager.GetOrNullAsync(SettingNames.TwoFactorMandatoryForRoles);

        if (string.IsNullOrWhiteSpace(mandatoryRolesString))
            return false;

        var mandatoryRoles = mandatoryRolesString.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(r => r.Trim())
            .ToList();

        foreach (var role in mandatoryRoles)
        {
            if (_currentUser.IsInRole(role))
            {
                return true;
            }
        }

        return false;
    }

    // Authenticator Methods
    public async Task<AuthenticatorSetupDto> SetupAuthenticatorAsync()
    {
        if (!_currentUser.Id.HasValue)
            throw new UnauthorizedAccessException("User not authenticated");

        var userId = _currentUser.Id.Value;
        var issuer = "YourApp"; // TODO: Make this configurable
        var accountName = _currentUser.Email ?? _currentUser.UserName ?? userId.ToString();

        var (secret, qrCodeUri) = await _twoFactorService.GenerateAuthenticatorSecretAsync(userId, issuer, accountName);

        return new AuthenticatorSetupDto
        {
            Secret = secret,
            QrCodeUri = qrCodeUri,
            ManualEntryKey = secret
        };
    }

    public async Task<bool> EnableAuthenticatorAsync(EnableAuthenticatorDto input)
    {
        if (!_currentUser.Id.HasValue)
            throw new UnauthorizedAccessException("User not authenticated");

        var userId = _currentUser.Id.Value;
        return await _twoFactorService.EnableAuthenticatorAsync(userId, input.VerificationCode);
    }

    public async Task DisableAuthenticatorAsync()
    {
        if (!_currentUser.Id.HasValue)
            throw new UnauthorizedAccessException("User not authenticated");

        var userId = _currentUser.Id.Value;
        await _twoFactorService.DisableAuthenticatorAsync(userId);
    }

    // Phone Verification Methods
    public async Task SendPhoneVerificationCodeAsync()
    {
        if (!_currentUser.Id.HasValue)
            throw new UnauthorizedAccessException("User not authenticated");

        var userId = _currentUser.Id.Value;
        
        // Generate and store verification code
        await _twoFactorService.SendPhoneVerificationCodeAsync(userId);

        // Get the code and send via SMS
        // Note: This requires SmsSenderFactory to be injected
        // For now, we'll leave this as a TODO for the caller to handle
    }

    public async Task<bool> VerifyPhoneNumberAsync(VerifyPhoneDto input)
    {
        if (!_currentUser.Id.HasValue)
            throw new UnauthorizedAccessException("User not authenticated");

        var userId = _currentUser.Id.Value;
        return await _twoFactorService.VerifyPhoneNumberAsync(userId, input.Code);
    }

    // Provider Management
    public async Task<string> GetPreferredProviderAsync()
    {
        if (!_currentUser.Id.HasValue)
            throw new UnauthorizedAccessException("User not authenticated");

        var userId = _currentUser.Id.Value;
        return await _twoFactorService.GetPreferredProviderAsync(userId);
    }

    public async Task SetPreferredProviderAsync(SetPreferredProviderDto input)
    {
        if (!_currentUser.Id.HasValue)
            throw new UnauthorizedAccessException("User not authenticated");

        var userId = _currentUser.Id.Value;
        await _twoFactorService.SetPreferredProviderAsync(userId, input.Provider);
    }
}
