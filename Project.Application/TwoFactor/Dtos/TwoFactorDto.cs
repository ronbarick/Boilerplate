using System;
using System.Collections.Generic;
using Project.Application.Common.Dtos;

namespace Project.Application.TwoFactor.Dtos;

public class VerifyOtpDto
{
    public Guid UserId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Provider { get; set; } // "Email", "Sms", "Authenticator"
    public bool TrustDevice { get; set; } = false;
}

public class TwoFactorStatusDto
{
    public bool IsEnabled { get; set; }
    public bool IsMandatory { get; set; }
    public int BackupCodesRemaining { get; set; }
    public int TrustedDevicesCount { get; set; }
    public string PreferredProvider { get; set; } = "Email";
    public bool IsPhoneVerified { get; set; }
    public bool IsAuthenticatorEnabled { get; set; }
}

public class BackupCodesDto
{
    public List<string> Codes { get; set; } = new();
}

public class TrustedDeviceDto
{
    public Guid Id { get; set; }
    public string DeviceName { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}

public class ResendOtpDto
{
    public Guid UserId { get; set; }
}

public class AuthenticatorSetupDto
{
    public string Secret { get; set; } = string.Empty;
    public string QrCodeUri { get; set; } = string.Empty;
    public string ManualEntryKey { get; set; } = string.Empty;
}

public class EnableAuthenticatorDto
{
    public string VerificationCode { get; set; } = string.Empty;
}

public class VerifyPhoneDto
{
    public string Code { get; set; } = string.Empty;
}

public class SetPreferredProviderDto
{
    public string Provider { get; set; } = string.Empty; // "Email", "Sms", "Authenticator"
}
