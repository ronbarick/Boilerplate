using Project.Core.Entities.Base;
using Project.Core.Enums;

namespace Project.Core.Entities.SaaS;

/// <summary>
/// Payment transaction for a subscription
/// </summary>
public class SaasTenantSubscriptionPayment : FullAuditedEntity
{
    /// <summary>
    /// Subscription ID
    /// </summary>
    public Guid SubscriptionId { get; set; }
    
    /// <summary>
    /// Payment gateway used
    /// </summary>
    public PaymentGateway Gateway { get; set; }
    
    /// <summary>
    /// Gateway order ID
    /// </summary>
    public string GatewayOrderId { get; set; } = null!;
    
    /// <summary>
    /// Gateway payment ID (after successful payment)
    /// </summary>
    public string? GatewayPaymentId { get; set; }
    
    /// <summary>
    /// Payment amount
    /// </summary>
    public decimal Amount { get; set; }
    
    /// <summary>
    /// Currency code (ISO 4217)
    /// </summary>
    public string Currency { get; set; } = "INR";
    
    /// <summary>
    /// Payment status
    /// </summary>
    public PaymentStatus Status { get; set; }
    
    /// <summary>
    /// Payment completion date
    /// </summary>
    public DateTime? PaymentDate { get; set; }
    
    /// <summary>
    /// Failure reason if payment failed
    /// </summary>
    public string? FailureReason { get; set; }
    
    /// <summary>
    /// Raw webhook data (JSON)
    /// </summary>
    public string? WebhookData { get; set; }
    
    /// <summary>
    /// Proration amount (for upgrades/downgrades)
    /// </summary>
    public decimal? ProrationAmount { get; set; }
    
    /// <summary>
    /// Proration reason/explanation
    /// </summary>
    public string? ProrationReason { get; set; }
    
    /// <summary>
    /// Number of retry attempts
    /// </summary>
    public int RetryCount { get; set; }
    
    /// <summary>
    /// Next scheduled retry date
    /// </summary>
    public DateTime? NextRetryDate { get; set; }
    
    /// <summary>
    /// Retry history (JSON array)
    /// </summary>
    public string? RetryHistory { get; set; }
    
    /// <summary>
    /// Navigation property to subscription
    /// </summary>
    public SaasTenantSubscription Subscription { get; set; } = null!;
}
