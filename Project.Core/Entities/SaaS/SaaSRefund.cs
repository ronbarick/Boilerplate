using Project.Core.Entities.Base;
using Project.Core.Enums;

namespace Project.Core.Entities.SaaS;

/// <summary>
/// Refund for a subscription payment
/// </summary>
public class SaaSRefund : FullAuditedEntity
{
    /// <summary>
    /// Payment ID
    /// </summary>
    public Guid PaymentId { get; set; }
    
    /// <summary>
    /// Refund amount
    /// </summary>
    public decimal RefundAmount { get; set; }
    
    /// <summary>
    /// Reason for refund
    /// </summary>
    public string RefundReason { get; set; } = null!;
    
    /// <summary>
    /// Refund status
    /// </summary>
    public RefundStatus RefundStatus { get; set; }
    
    /// <summary>
    /// Gateway refund ID
    /// </summary>
    public string? GatewayRefundId { get; set; }
    
    /// <summary>
    /// Refund completion date
    /// </summary>
    public DateTime? RefundDate { get; set; }
    
    /// <summary>
    /// Admin user who processed the refund
    /// </summary>
    public Guid? RefundedBy { get; set; }
    
    /// <summary>
    /// Navigation property to payment
    /// </summary>
    public SaasTenantSubscriptionPayment Payment { get; set; } = null!;
}
