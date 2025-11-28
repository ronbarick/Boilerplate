using Project.Core.Entities.Base;

namespace Project.Core.Entities.SaaS;

/// <summary>
/// Coupon applied to a tenant's subscription
/// </summary>
public class SaasTenantCoupon : FullAuditedEntity
{
    /// <summary>
    /// Tenant ID
    /// </summary>
    public new Guid TenantId { get; set; }
    
    /// <summary>
    /// Coupon ID
    /// </summary>
    public Guid CouponId { get; set; }
    
    /// <summary>
    /// Subscription ID
    /// </summary>
    public Guid SubscriptionId { get; set; }
    
    /// <summary>
    /// Date when coupon was applied
    /// </summary>
    public DateTime AppliedDate { get; set; }
    
    /// <summary>
    /// Actual discount amount applied
    /// </summary>
    public decimal DiscountAmount { get; set; }
    
    /// <summary>
    /// Navigation property to coupon
    /// </summary>
    public SaasCoupon Coupon { get; set; } = null!;
    
    /// <summary>
    /// Navigation property to subscription
    /// </summary>
    public SaasTenantSubscription Subscription { get; set; } = null!;
}
