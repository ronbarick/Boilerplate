using Project.Core.Entities.Base;
using Project.Core.Enums;

namespace Project.Core.Entities.SaaS;

/// <summary>
/// Webhook processing log
/// </summary>
public class SaaSWebhookLog : FullAuditedEntity
{
    /// <summary>
    /// Event ID from the gateway
    /// </summary>
    public string? EventId { get; set; }

    /// <summary>
    /// Event type (e.g., invoice.paid)
    /// </summary>
    public string? EventType { get; set; }

    /// <summary>
    /// Payment gateway
    /// </summary>
    public PaymentGateway Gateway { get; set; }
    
    /// <summary>
    /// Raw webhook data (JSON)
    /// </summary>
    public string WebhookData { get; set; } = null!;
    
    /// <summary>
    /// Whether webhook was successfully processed
    /// </summary>
    public bool IsProcessed { get; set; }
    
    /// <summary>
    /// Number of retry attempts
    /// </summary>
    public int RetryCount { get; set; }
    
    /// <summary>
    /// Error message if processing failed
    /// </summary>
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// Processing completion date
    /// </summary>
    public DateTime? ProcessedDate { get; set; }
}
