using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Project.Domain.Entities;

namespace Project.Domain.Interfaces;

/// <summary>
/// Service for managing two-factor authentication.
/// </summary>
public interface ITwoFactorService
{
    /// <summary>
    /// Generates a 4-digit OTP code for the user.
    /// </summary>
    Task<string> GenerateOtpAsync(Guid userId);

    /// <summary>
    /// Validates an OTP code for the user.
    /// </summary>
    Task<bool> ValidateOtpAsync(Guid userId, string code);

    /// <summary>
    /// Validates a backup code for the user.
    /// </summary>
    Task<bool> ValidateBackupCodeAsync(Guid userId, string code);

    /// <summary>
    /// Generates 10 backup codes for the user and returns them in plain text.
    /// Previous backup codes are invalidated.
    /// </summary>
    Task<List<string>> GenerateBackupCodesAsync(Guid userId);

    /// <summary>
    /// Checks if two-factor authentication is required for the user.
    /// Considers settings hierarchy, mandatory roles, and trusted devices.
    /// </summary>
    Task<bool> IsTwoFactorRequiredAsync(Guid userId, Guid? tenantId, string[] roles, string? deviceFingerprint = null);

    /// <summary>
    /// Checks if the device is trusted for the user.
    /// </summary>
    Task<bool> IsTrustedDeviceAsync(Guid userId, string deviceFingerprint);

    /// <summary>
    /// Adds a trusted device for the user.
    /// </summary>
    Task AddTrustedDeviceAsync(Guid userId, string deviceFingerprint, string deviceName);

    /// <summary>
    /// Gets all trusted devices for the user.
    /// </summary>
    Task<List<TrustedDevice>> GetTrustedDevicesAsync(Guid userId);

    /// <summary>
    /// Revokes a trusted device.
    /// </summary>
    Task RevokeTrustedDeviceAsync(Guid userId, Guid deviceId);

    /// <summary>
    /// Checks if the user can request a new OTP (rate limiting).
    /// </summary>
    Task<bool> CanRequestOtpAsync(Guid userId);

    /// <summary>
    /// Checks if the user can verify an OTP (rate limiting).
    /// </summary>
    Task<bool> CanVerifyOtpAsync(Guid userId);

    /// <summary>
    /// Invalidates all OTP codes for the user.
    /// </summary>
    Task InvalidateOtpAsync(Guid userId);

    /// <summary>
    /// Gets the count of remaining backup codes for the user.
    /// </summary>
    Task<int> GetRemainingBackupCodesCountAsync(Guid userId);

    // SMS OTP Methods
    /// <summary>
    /// Generates and sends an SMS OTP code for the user.
    /// </summary>
    Task<string> GenerateSmsOtpAsync(Guid userId);

    /// <summary>
    /// Validates an SMS OTP code for the user.
    /// </summary>
    Task<bool> ValidateSmsOtpAsync(Guid userId, string code);

    // Authenticator (TOTP) Methods
    /// <summary>
    /// Generates a new authenticator secret and QR code URI for the user.
    /// </summary>
    Task<(string Secret, string QrCodeUri)> GenerateAuthenticatorSecretAsync(Guid userId, string issuer, string accountName);

    /// <summary>
    /// Enables authenticator for the user after verifying the setup code.
    /// </summary>
    Task<bool> EnableAuthenticatorAsync(Guid userId, string verificationCode);

    /// <summary>
    /// Disables authenticator for the user.
    /// </summary>
    Task DisableAuthenticatorAsync(Guid userId);

    /// <summary>
    /// Validates a TOTP code for the user.
    /// </summary>
    Task<bool> ValidateTotpAsync(Guid userId, string code);

    // Phone Verification Methods
    /// <summary>
    /// Sends a verification code to the user's phone number.
    /// </summary>
    Task SendPhoneVerificationCodeAsync(Guid userId);

    /// <summary>
    /// Verifies the user's phone number with the provided code.
    /// </summary>
    Task<bool> VerifyPhoneNumberAsync(Guid userId, string code);

    // Provider Management
    /// <summary>
    /// Gets the user's preferred 2FA provider.
    /// </summary>
    Task<string> GetPreferredProviderAsync(Guid userId);

    /// <summary>
    /// Sets the user's preferred 2FA provider.
    /// </summary>
    Task SetPreferredProviderAsync(Guid userId, string provider);
}
