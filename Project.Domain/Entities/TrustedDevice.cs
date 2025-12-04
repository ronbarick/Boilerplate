using System;
using Project.Domain.Entities.Base;

namespace Project.Domain.Entities;

/// <summary>
/// Trusted devices that can skip two-factor authentication.
/// Inherits from FullAuditedEntity for audit trail.
/// </summary>
public class TrustedDevice : FullAuditedEntity
{
    public Guid UserId { get; set; }
    public string DeviceFingerprint { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime? LastUsedAt { get; set; }
}
