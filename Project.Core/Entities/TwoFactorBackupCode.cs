using System;
using Project.Core.Entities.Base;

namespace Project.Core.Entities;

/// <summary>
/// Backup codes for two-factor authentication recovery.
/// Inherits from FullAuditedEntity for audit trail.
/// </summary>
public class TwoFactorBackupCode : FullAuditedEntity
{
    public Guid UserId { get; set; }
    public string CodeHash { get; set; } = string.Empty;
    public bool IsUsed { get; set; } = false;
    public DateTime? UsedAt { get; set; }
}
