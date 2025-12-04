using Project.Domain.Entities.Base;


namespace Project.Domain.Entities.SaaS;

/// <summary>
/// Invoice for subscription payment
/// </summary>
public class SaaSInvoice : FullAuditedEntity
{
    /// <summary>
    /// Unique invoice number
    /// </summary>
    public string InvoiceNumber { get; set; } = null!;
    
    /// <summary>
    /// Tenant ID
    /// </summary>
    public new Guid TenantId { get; set; }
    
    /// <summary>
    /// Subscription ID
    /// </summary>
    public Guid SubscriptionId { get; set; }
    
    /// <summary>
    /// Payment ID (if paid)
    /// </summary>
    public Guid? PaymentId { get; set; }
    
    /// <summary>
    /// Base amount
    /// </summary>
    public decimal Amount { get; set; }
    
    /// <summary>
    /// Tax amount
    /// </summary>
    public decimal Tax { get; set; }
    
    /// <summary>
    /// Total amount (Amount + Tax)
    /// </summary>
    public decimal TotalAmount { get; set; }
    
    /// <summary>
    /// Currency code
    /// </summary>
    public string Currency { get; set; } = "INR";
    
    /// <summary>
    /// Invoice status
    /// </summary>
    public InvoiceStatus Status { get; set; }
    
    /// <summary>
    /// Invoice issue date
    /// </summary>
    public DateTime IssuedDate { get; set; }
    
    /// <summary>
    /// Payment due date
    /// </summary>
    public DateTime DueDate { get; set; }
    
    /// <summary>
    /// Payment completion date
    /// </summary>
    public DateTime? PaidDate { get; set; }
    
    /// <summary>
    /// PDF file path
    /// </summary>
    public string? PdfPath { get; set; }
    
    /// <summary>
    /// Navigation property to subscription
    /// </summary>
    public SaasTenantSubscription Subscription { get; set; } = null!;
    
    /// <summary>
    /// Navigation property to payment
    /// </summary>
    public SaasTenantSubscriptionPayment? Payment { get; set; }
}
