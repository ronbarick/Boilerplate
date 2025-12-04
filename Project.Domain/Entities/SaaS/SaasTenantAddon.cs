using Project.Domain.Entities.Base;

namespace Project.Domain.Entities.SaaS;

/// <summary>
/// Tenant's purchased addon
/// </summary>
public class SaasTenantAddon : FullAuditedEntity
{
    /// <summary>
    /// Tenant ID
    /// </summary>
    public new Guid TenantId { get; set; }
    
    /// <summary>
    /// Addon ID
    /// </summary>
    public Guid AddonId { get; set; }
    
    /// <summary>
    /// Associated subscription ID (optional)
    /// </summary>
    public Guid? SubscriptionId { get; set; }
    
    /// <summary>
    /// Purchase date
    /// </summary>
    public DateTime PurchaseDate { get; set; }
    
    /// <summary>
    /// Expiry date (null = lifetime)
    /// </summary>
    public DateTime? ExpiryDate { get; set; }
    
    /// <summary>
    /// Whether addon is currently active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Navigation property to addon
    /// </summary>
    public SaaSAddon Addon { get; set; } = null!;
    
    /// <summary>
    /// Navigation property to subscription
    /// </summary>
    public SaasTenantSubscription? Subscription { get; set; }
}
