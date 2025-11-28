using System;
using System.Collections.Generic;
using Project.Core.Enums;
using Project.Application.Common.Dtos;

namespace Project.Application.SaaS.Dtos;

#region Plans

public class SaasPlanDto : FullAuditedEntityDto<Guid>
{
    public string Name { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string? Description { get; set; }
    public BillingCycle BillingCycle { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "INR";
    public bool IsFree { get; set; }
    public int TrialDays { get; set; }
    public bool IsActive { get; set; }
    public string? Color { get; set; }
    public int SortOrder { get; set; }
    public List<SaasPlanFeatureDto> Features { get; set; } = new();
}

public class CreateSaasPlanDto
{
    public string Name { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string? Description { get; set; }
    public BillingCycle BillingCycle { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "INR";
    public bool IsFree { get; set; }
    public int TrialDays { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Color { get; set; }
    public int SortOrder { get; set; }
    public List<CreateSaasPlanFeatureDto> Features { get; set; } = new();
}

public class UpdateSaasPlanDto
{
    public string DisplayName { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
    public string? Color { get; set; }
    public int SortOrder { get; set; }
}

public class SaasPlanFeatureDto : EntityDto<Guid>
{
    public string FeatureName { get; set; } = null!;
    public string FeatureValue { get; set; } = null!;
    public FeatureType FeatureType { get; set; }
}

public class CreateSaasPlanFeatureDto
{
    public string FeatureName { get; set; } = null!;
    public string FeatureValue { get; set; } = null!;
    public FeatureType FeatureType { get; set; }
}

#endregion

#region Subscriptions

public class SaasTenantSubscriptionDto : FullAuditedEntityDto<Guid>
{
    public Guid TenantId { get; set; }
    public Guid PlanId { get; set; }
    public string PlanName { get; set; } = null!;
    public SubscriptionStatus Status { get; set; }
    public bool IsCurrentSubscription { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? TrialEndDate { get; set; }
    public bool AutoRenew { get; set; }
    public int GracePeriodDays { get; set; }
    public DateTime? CancellationDate { get; set; }
    public string? CancellationReason { get; set; }
    public CancellationType? CancellationType { get; set; }
    public DateTime? PausedDate { get; set; }
    public string? PauseReason { get; set; }
}

public class CreateSubscriptionDto
{
    public Guid PlanId { get; set; }
    public BillingCycle BillingCycle { get; set; }
    public string? CouponCode { get; set; }
}

public class SubscriptionHistoryDto : EntityDto<Guid>
{
    public Guid PlanId { get; set; }
    public string PlanName { get; set; } = null!;
    public SubscriptionStatus Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal TotalPaid { get; set; }
}

#endregion

#region Payments

public class SaasTenantSubscriptionPaymentDto : FullAuditedEntityDto<Guid>
{
    public Guid SubscriptionId { get; set; }
    public PaymentGateway Gateway { get; set; }
    public string GatewayOrderId { get; set; } = null!;
    public string? GatewayPaymentId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = null!;
    public PaymentStatus Status { get; set; }
    public DateTime? PaymentDate { get; set; }
    public string? FailureReason { get; set; }
    public decimal? ProrationAmount { get; set; }
    public string? ProrationReason { get; set; }
    public bool IsRefunded { get; set; }
}

public class PaymentMethodDto : FullAuditedEntityDto<Guid>
{
    public PaymentGateway Gateway { get; set; }
    public string Last4Digits { get; set; } = null!;
    public string? CardBrand { get; set; }
    public int? ExpiryMonth { get; set; }
    public int? ExpiryYear { get; set; }
    public bool IsDefault { get; set; }
}

#endregion

#region Invoices

public class SaaSInvoiceDto : FullAuditedEntityDto<Guid>
{
    public string InvoiceNumber { get; set; } = null!;
    public decimal Amount { get; set; }
    public decimal Tax { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = null!;
    public InvoiceStatus Status { get; set; }
    public DateTime IssuedDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? PaidDate { get; set; }
    public string? PdfUrl { get; set; }
}

#endregion

#region Features

public class SaaSFeatureDto : FullAuditedEntityDto<Guid>
{
    public string Name { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string? Description { get; set; }
    public string DefaultValue { get; set; } = null!;
    public FeatureType FeatureType { get; set; }
    public string? GroupName { get; set; }
    public int? AlertThresholdPercentage { get; set; }
}

public class UpdateFeatureDto
{
    public string DisplayName { get; set; } = null!;
    public string? Description { get; set; }
    public string DefaultValue { get; set; } = null!;
    public int? AlertThresholdPercentage { get; set; }
}

#endregion

#region Coupons

public class SaasCouponDto : FullAuditedEntityDto<Guid>
{
    public string Code { get; set; } = null!;
    public string? Description { get; set; }
    public DiscountType DiscountType { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public decimal? DiscountAmount { get; set; }
    public string? Currency { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public int? MaxUses { get; set; }
    public int UsedCount { get; set; }
    public bool IsActive { get; set; }
    public List<Guid>? ApplicablePlanIds { get; set; }
}

public class CreateSaasCouponDto
{
    public string Code { get; set; } = null!;
    public string? Description { get; set; }
    public DiscountType DiscountType { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public decimal? DiscountAmount { get; set; }
    public string? Currency { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public int? MaxUses { get; set; }
    public List<Guid>? ApplicablePlanIds { get; set; }
}

#endregion

#region Addons

public class SaaSAddonDto : FullAuditedEntityDto<Guid>
{
    public string Name { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = null!;
    public bool IsRecurring { get; set; }
    public BillingCycle? BillingCycle { get; set; }
    public bool IsActive { get; set; }
}

public class CreateSaaSAddonDto
{
    public string Name { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "INR";
    public bool IsRecurring { get; set; }
    public BillingCycle? BillingCycle { get; set; }
}

#endregion

#region Analytics

public class SubscriptionMetricsDto
{
    public int TotalSubscribers { get; set; }
    public int ActiveSubscribers { get; set; }
    public int TrialSubscribers { get; set; }
    public int ChurnedSubscribers { get; set; }
    public decimal MonthlyRecurringRevenue { get; set; }
    public decimal TotalRevenue { get; set; }
}

public class RevenueReportDto
{
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public int TransactionCount { get; set; }
}

public class SaaSAnalyticsDto
{
    public int TotalSubscriptions { get; set; }
    public int ActiveSubscriptions { get; set; }
    public int TrialSubscriptions { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal MonthlyRevenue { get; set; }
}

#endregion
