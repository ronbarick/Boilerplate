using Project.Core.Entities.Base;

namespace Project.Core.Entities.SaaS;

/// <summary>
/// Audit trail for subscription changes
/// </summary>
public class SaaSSubscriptionAudit : FullAuditedEntity
{
    /// <summary>
    /// Subscription ID
    /// </summary>
    public Guid SubscriptionId { get; set; }
    
    /// <summary>
    /// Action performed (Created, Upgraded, Cancelled, etc.)
    /// </summary>
    public string Action { get; set; } = null!;
    
    /// <summary>
    /// Old value (JSON)
    /// </summary>
    public string? OldValue { get; set; }
    
    /// <summary>
    /// New value (JSON)
    /// </summary>
    public string? NewValue { get; set; }
    
    /// <summary>
    /// User who performed the action
    /// </summary>
    public Guid? PerformedBy { get; set; }
    
    /// <summary>
    /// IP address of the request
    /// </summary>
    public string? IpAddress { get; set; }
    
    /// <summary>
    /// Navigation property to subscription
    /// </summary>
    public SaasTenantSubscription Subscription { get; set; } = null!;
}
