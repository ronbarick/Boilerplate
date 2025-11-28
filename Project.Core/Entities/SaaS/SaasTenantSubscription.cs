using Project.Core.Entities.Base;
using Project.Core.Enums;

namespace Project.Core.Entities.SaaS;

/// <summary>
/// Tenant's active subscription
/// </summary>
public class SaasTenantSubscription : FullAuditedEntity
{
    /// <summary>
    /// Tenant ID (one subscription per tenant)
    /// </summary>
    public new Guid TenantId { get; set; }
    
    /// <summary>
    /// Subscribed plan ID
    /// </summary>
    public Guid PlanId { get; set; }
    
    /// <summary>
    /// Current subscription status
    /// </summary>
    public SubscriptionStatus Status { get; set; }
    
    /// <summary>
    /// Whether this is the current active subscription
    /// </summary>
    public bool IsCurrentSubscription { get; set; } = true;
    
    /// <summary>
    /// Subscription start date
    /// </summary>
    public DateTime StartDate { get; set; }
    
    /// <summary>
    /// Subscription end date (null = lifetime)
    /// </summary>
    public DateTime? EndDate { get; set; }
    
    /// <summary>
    /// Trial period end date
    /// </summary>
    public DateTime? TrialEndDate { get; set; }
    
    /// <summary>
    /// Number of times trial has been extended
    /// </summary>
    public int TrialExtensionCount { get; set; }
    
    /// <summary>
    /// Trial extension history (JSON array)
    /// </summary>
    public string? TrialExtensionHistory { get; set; }
    
    /// <summary>
    /// Whether subscription should auto-renew
    /// </summary>
    public bool AutoRenew { get; set; } = true;
    
    /// <summary>
    /// Grace period in days after expiry
    /// </summary>
    public int GracePeriodDays { get; set; }
    
    /// <summary>
    /// Cancellation date
    /// </summary>
    public DateTime? CancellationDate { get; set; }
    
    /// <summary>
    /// Reason for cancellation
    /// </summary>
    public string? CancellationReason { get; set; }
    
    /// <summary>
    /// Type of cancellation
    /// </summary>
    public CancellationType? CancellationType { get; set; }
    
    /// <summary>
    /// Date when subscription was paused
    /// </summary>
    public DateTime? PausedDate { get; set; }
    
    /// <summary>
    /// Reason for pausing
    /// </summary>
    public string? PauseReason { get; set; }
    
    /// <summary>
    /// Billing address
    /// </summary>
    public string? BillingAddress { get; set; }
    
    /// <summary>
    /// Billing city
    /// </summary>
    public string? BillingCity { get; set; }
    
    /// <summary>
    /// Billing state/province
    /// </summary>
    public string? BillingState { get; set; }
    
    /// <summary>
    /// Billing country
    /// </summary>
    public string? BillingCountry { get; set; }
    
    /// <summary>
    /// Billing zip/postal code
    /// </summary>
    public string? BillingZipCode { get; set; }
    
    /// <summary>
    /// Tax ID / VAT / GST number
    /// </summary>
    public string? TaxId { get; set; }
    
    /// <summary>
    /// Company name for billing
    /// </summary>
    public string? CompanyName { get; set; }
    
    /// <summary>
    /// Navigation property to plan
    /// </summary>
    public SaasPlan Plan { get; set; } = null!;
    
    /// <summary>
    /// Payment history for this subscription
    /// </summary>
    public ICollection<SaasTenantSubscriptionPayment> Payments { get; set; } = new List<SaasTenantSubscriptionPayment>();
}
