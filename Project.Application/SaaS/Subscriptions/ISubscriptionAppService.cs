using System;
using System.Threading.Tasks;
using Project.Application.SaaS.Dtos;
using Project.Domain.Interfaces;

namespace Project.Application.SaaS.Subscriptions;

public interface ISubscriptionAppService : IApplicationService
{
    Task<SaasTenantSubscriptionDto> GetCurrentSubscriptionAsync(Guid tenantId);
    Task<SaasTenantSubscriptionDto> SubscribeAsync(Guid tenantId, Guid planId);
    Task<SaasTenantSubscriptionDto> UpgradeSubscriptionAsync(Guid tenantId, Guid newPlanId);
    Task<SaasTenantSubscriptionDto> DowngradeSubscriptionAsync(Guid tenantId, Guid newPlanId);
    Task CancelSubscriptionAsync(Guid tenantId, string reason);
}
