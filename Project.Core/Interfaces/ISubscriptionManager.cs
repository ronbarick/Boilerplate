using Project.Core.Entities.SaaS;
using Project.Core.Enums;

namespace Project.Core.Interfaces;

public interface ISubscriptionManager
{
    Task<SaasTenantSubscription> CreateSubscriptionAsync(Guid tenantId, Guid planId, BillingCycle billingCycle);
    Task<SaasTenantSubscription> UpgradeSubscriptionAsync(Guid tenantId, Guid newPlanId);
    Task<SaasTenantSubscription> DowngradeSubscriptionAsync(Guid tenantId, Guid newPlanId);
    Task CancelSubscriptionAsync(Guid tenantId, string reason, CancellationType type);
    Task PauseSubscriptionAsync(Guid tenantId, string reason);
    Task ResumeSubscriptionAsync(Guid tenantId);
    Task ExtendTrialAsync(Guid tenantId, int days);
    Task<bool> IsSubscriptionActiveAsync(Guid tenantId);
    Task CheckExpirationsAsync();
}
