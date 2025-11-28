using Project.Core.Entities.Base;
using Project.Core.Enums;

namespace Project.Core.Entities.SaaS;

/// <summary>
/// Saved payment method for recurring payments
/// </summary>
public class SaasTenantPaymentMethod : FullAuditedEntity
{
    /// <summary>
    /// Tenant ID
    /// </summary>
    public new Guid TenantId { get; set; }
    
    /// <summary>
    /// Payment gateway
    /// </summary>
    public PaymentGateway Gateway { get; set; }
    
    /// <summary>
    /// Gateway customer ID (Razorpay/Stripe customer)
    /// </summary>
    public string GatewayCustomerId { get; set; } = null!;
    
    /// <summary>
    /// Gateway payment method ID (tokenized card/UPI)
    /// </summary>
    public string GatewayPaymentMethodId { get; set; } = null!;
    
    /// <summary>
    /// Last 4 digits of card (for display)
    /// </summary>
    public string? Last4Digits { get; set; }
    
    /// <summary>
    /// Card brand (Visa, Mastercard, etc.)
    /// </summary>
    public string? CardBrand { get; set; }
    
    /// <summary>
    /// Card expiry month
    /// </summary>
    public int? ExpiryMonth { get; set; }
    
    /// <summary>
    /// Card expiry year
    /// </summary>
    public int? ExpiryYear { get; set; }
    
    /// <summary>
    /// Payment method type (card, upi, netbanking)
    /// </summary>
    public string PaymentMethodType { get; set; } = "card";
    
    /// <summary>
    /// Whether this is the default payment method
    /// </summary>
    public bool IsDefault { get; set; }
    
    /// <summary>
    /// Whether this payment method is active
    /// </summary>
    public bool IsActive { get; set; } = true;
}
