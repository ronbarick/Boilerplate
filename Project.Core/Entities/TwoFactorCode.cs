using System;

namespace Project.Core.Entities;

/// <summary>
/// Temporary storage for OTP codes during two-factor authentication.
/// Does NOT inherit from FullAuditedEntity as this is temporary data.
/// </summary>
public class TwoFactorCode
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Code { get; set; } = null!;
    public string? Provider { get; set; } // "Email", "Sms", "PhoneVerification"
    public bool IsUsed { get; set; } = false;
    public int AttemptCount { get; set; } = 0;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}
