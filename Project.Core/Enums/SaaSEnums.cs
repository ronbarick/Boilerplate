namespace Project.Core.Enums;

/// <summary>
/// Billing cycle for subscription plans
/// </summary>
public enum BillingCycle
{
    Monthly = 1,
    Yearly = 2
}

/// <summary>
/// Subscription status lifecycle
/// </summary>
public enum SubscriptionStatus
{
    Trial = 1,
    Active = 2,
    Expired = 3,
    Cancelled = 4,
    Suspended = 5,
    Paused = 6
}

/// <summary>
/// Supported payment gateways
/// </summary>
public enum PaymentGateway
{
    Razorpay = 1,
    Stripe = 2
}

/// <summary>
/// Payment transaction status
/// </summary>
public enum PaymentStatus
{
    Pending = 1,
    Success = 2,
    Failed = 3,
    Refunded = 4
}

/// <summary>
/// Invoice status
/// </summary>
public enum InvoiceStatus
{
    Draft = 1,
    Paid = 2,
    Cancelled = 3
}

/// <summary>
/// Refund status
/// </summary>
public enum RefundStatus
{
    Pending = 1,
    Completed = 2,
    Failed = 3
}

/// <summary>
/// Feature types for subscription plans
/// </summary>
public enum FeatureType
{
    /// <summary>
    /// Boolean feature (enabled/disabled)
    /// </summary>
    Boolean = 1,
    
    /// <summary>
    /// Numeric feature with limits (e.g., MaxProjects = 100)
    /// </summary>
    Numeric = 2,
    
    /// <summary>
    /// Metered feature with usage tracking (e.g., API calls per month)
    /// </summary>
    Metered = 3
}

/// <summary>
/// Entity type for feature value assignment
/// </summary>
public enum FeatureEntityType
{
    /// <summary>
    /// Feature assigned to a tenant (override)
    /// </summary>
    Tenant = 1,
    
    /// <summary>
    /// Feature assigned to a plan
    /// </summary>
    Plan = 2
}

/// <summary>
/// Subscription cancellation types
/// </summary>
public enum CancellationType
{
    /// <summary>
    /// Cancel immediately with prorated refund
    /// </summary>
    Immediate = 1,
    
    /// <summary>
    /// Cancel at end of current billing cycle
    /// </summary>
    EndOfCycle = 2,
    
    /// <summary>
    /// Cancel on a specific scheduled date
    /// </summary>
    Scheduled = 3
}

/// <summary>
/// Coupon discount types
/// </summary>
public enum DiscountType
{
    /// <summary>
    /// Percentage discount (e.g., 20% off)
    /// </summary>
    Percentage = 1,
    
    /// <summary>
    /// Fixed amount discount (e.g., $10 off)
    /// </summary>
    FixedAmount = 2
}
