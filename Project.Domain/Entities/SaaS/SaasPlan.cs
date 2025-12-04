using Project.Domain.Entities.Base;


namespace Project.Domain.Entities.SaaS;

/// <summary>
/// Subscription plan definition
/// </summary>
public class SaasPlan : FullAuditedEntity
{
    /// <summary>
    /// Unique plan identifier (e.g., "starter", "professional")
    /// </summary>
    public string Name { get; set; } = null!;
    
    /// <summary>
    /// Display name for UI
    /// </summary>
    public string DisplayName { get; set; } = null!;
    
    /// <summary>
    /// Plan description
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Billing cycle (Monthly/Yearly)
    /// </summary>
    public BillingCycle BillingCycle { get; set; }
    
    /// <summary>
    /// Plan price
    /// </summary>
    public decimal Price { get; set; }
    
    /// <summary>
    /// Currency code (ISO 4217)
    /// </summary>
    public string Currency { get; set; } = "INR";
    
    /// <summary>
    /// Whether this is a free plan
    /// </summary>
    public bool IsFree { get; set; }
    
    /// <summary>
    /// Number of trial days (0 = no trial)
    /// </summary>
    public int TrialDays { get; set; }
    
    /// <summary>
    /// Whether this plan is active and available for subscription
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Badge color for UI (e.g., "#4CAF50")
    /// </summary>
    public string? Color { get; set; }
    
    /// <summary>
    /// Display order
    /// </summary>
    public int SortOrder { get; set; }
    
    /// <summary>
    /// Features included in this plan
    /// </summary>
    public ICollection<SaasPlanFeature> Features { get; set; } = new List<SaasPlanFeature>();
}
