using System;
using Project.Core.Enums;
using Project.Application.Common.Dtos;

namespace Project.Application.SaaS.Dtos;

public class SaaSSubscriptionAuditDto : FullAuditedEntityDto<Guid>
{
    public Guid SubscriptionId { get; set; }
    public string Action { get; set; } = null!;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public Guid? PerformedBy { get; set; }
    public string? IpAddress { get; set; }
}

public class SaaSWebhookLogDto : FullAuditedEntityDto<Guid>
{
    public int Gateway { get; set; }
    public string WebhookData { get; set; } = null!;
    public bool IsProcessed { get; set; }
    public int RetryCount { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime? ProcessedDate { get; set; }
}

public class SaaSRefundDto : FullAuditedEntityDto<Guid>
{
    public Guid PaymentId { get; set; }
    public decimal RefundAmount { get; set; }
    public string RefundReason { get; set; } = null!;
    public int RefundStatus { get; set; }
    public string? GatewayRefundId { get; set; }
    public DateTime? RefundDate { get; set; }
    public Guid? RefundedBy { get; set; }
}
