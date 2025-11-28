using Project.Core.Entities.Base;
using Project.Core.Enums;

namespace Project.Core.Entities.SaaS;

/// <summary>
/// Coupon/discount code
/// </summary>
public class SaasCoupon : FullAuditedEntity
{
    /// <summary>
    /// Coupon code (unique)
    /// </summary>
    public string Code { get; set; } = null!;
    
    /// <summary>
    /// Coupon description
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Discount type (Percentage or Fixed Amount)
    /// </summary>
    public DiscountType DiscountType { get; set; }
    
    /// <summary>
    /// Discount percentage (if type is Percentage)
    /// </summary>
    public decimal? DiscountPercentage { get; set; }
    
    /// <summary>
    /// Discount amount (if type is FixedAmount)
    /// </summary>
    public decimal? DiscountAmount { get; set; }
    
    /// <summary>
    /// Currency for fixed amount discounts
    /// </summary>
    public string? Currency { get; set; }
    
    /// <summary>
    /// Valid from date
    /// </summary>
    public DateTime ValidFrom { get; set; }
    
    /// <summary>
    /// Valid to date
    /// </summary>
    public DateTime ValidTo { get; set; }
    
    /// <summary>
    /// Maximum number of uses (null = unlimited)
    /// </summary>
    public int? MaxUses { get; set; }
    
    /// <summary>
    /// Current usage count
    /// </summary>
    public int UsedCount { get; set; }
    
    /// <summary>
    /// Whether coupon is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Applicable plan IDs (JSON array, null = all plans)
    /// </summary>
    public string? ApplicablePlans { get; set; }
}
