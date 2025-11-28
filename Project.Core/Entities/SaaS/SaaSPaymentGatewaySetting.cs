using Project.Core.Entities.Base;
using Project.Core.Enums;

namespace Project.Core.Entities.SaaS;

/// <summary>
/// Payment gateway configuration
/// </summary>
public class SaaSPaymentGatewaySetting : FullAuditedEntity
{
    /// <summary>
    /// Payment gateway
    /// </summary>
    public PaymentGateway Gateway { get; set; }
    
    /// <summary>
    /// Whether this gateway is active
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// API key (should be encrypted)
    /// </summary>
    public string ApiKey { get; set; } = null!;
    
    /// <summary>
    /// API secret (should be encrypted)
    /// </summary>
    public string ApiSecret { get; set; } = null!;
    
    /// <summary>
    /// Webhook secret for signature verification (should be encrypted)
    /// </summary>
    public string WebhookSecret { get; set; } = null!;
    
    /// <summary>
    /// Whether to use test mode
    /// </summary>
    public bool TestMode { get; set; }
}
