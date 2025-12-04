using Project.Domain.Entities.Base;


namespace Project.Domain.Entities.SaaS;

/// <summary>
/// Add-on product/feature
/// </summary>
public class SaaSAddon : FullAuditedEntity
{
    /// <summary>
    /// Addon name (unique identifier)
    /// </summary>
    public string Name { get; set; } = null!;
    
    /// <summary>
    /// Display name for UI
    /// </summary>
    public string DisplayName { get; set; } = null!;
    
    /// <summary>
    /// Addon description
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Addon price
    /// </summary>
    public decimal Price { get; set; }
    
    /// <summary>
    /// Currency
    /// </summary>
    public string Currency { get; set; } = "INR";
    
    /// <summary>
    /// Whether this is a recurring charge
    /// </summary>
    public bool IsRecurring { get; set; }
    
    /// <summary>
    /// Billing cycle if recurring
    /// </summary>
    public BillingCycle? BillingCycle { get; set; }
    
    /// <summary>
    /// Whether addon is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Display order
    /// </summary>
    public int SortOrder { get; set; }
}
