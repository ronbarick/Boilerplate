using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project.Application.SaaS.Dtos;
using Project.Application.Services;
using Project.Core.Entities.SaaS;
using Project.Core.Enums;
using Project.Core.Interfaces;
using Project.Core.Interfaces.DependencyInjection;
using Project.Core.Localization;

namespace Project.Application.SaaS.Subscriptions;

public class SubscriptionAppService : AppServiceBase, ISubscriptionAppService
{
    private readonly ISubscriptionManager _subscriptionManager;
    private readonly IRepository<SaasTenantSubscription, Guid> _subscriptionRepository;
    private readonly IMapper _mapper;

    public SubscriptionAppService(
        ISubscriptionManager subscriptionManager,
        IRepository<SaasTenantSubscription, Guid> subscriptionRepository,
        IMapper mapper,
        ICurrentUser currentUser,
        ICurrentTenant currentTenant,
        IPermissionChecker permissionChecker,
        ILocalizationManager localizationManager)
        : base(currentUser, currentTenant, permissionChecker, localizationManager)
    {
        _subscriptionManager = subscriptionManager;
        _subscriptionRepository = subscriptionRepository;
        _mapper = mapper;
    }

    public async Task<SaasTenantSubscriptionDto> GetCurrentSubscriptionAsync(Guid tenantId)
    {
        var subscription = await _subscriptionRepository.GetQueryable()
            .Include(x => x.Plan)
            .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.IsCurrentSubscription);
            
        return _mapper.Map<SaasTenantSubscriptionDto>(subscription);
    }

    public async Task<SaasTenantSubscriptionDto> SubscribeAsync(Guid tenantId, Guid planId)
    {
        var subscription = await _subscriptionManager.CreateSubscriptionAsync(tenantId, planId, BillingCycle.Monthly);
        return _mapper.Map<SaasTenantSubscriptionDto>(subscription);
    }

    public async Task<SaasTenantSubscriptionDto> UpgradeSubscriptionAsync(Guid tenantId, Guid newPlanId)
    {
        var subscription = await _subscriptionManager.UpgradeSubscriptionAsync(tenantId, newPlanId);
        return _mapper.Map<SaasTenantSubscriptionDto>(subscription);
    }

    public async Task<SaasTenantSubscriptionDto> DowngradeSubscriptionAsync(Guid tenantId, Guid newPlanId)
    {
        var subscription = await _subscriptionManager.DowngradeSubscriptionAsync(tenantId, newPlanId);
        return _mapper.Map<SaasTenantSubscriptionDto>(subscription);
    }

    public async Task CancelSubscriptionAsync(Guid tenantId, string reason)
    {
        await _subscriptionManager.CancelSubscriptionAsync(tenantId, reason, CancellationType.EndOfCycle);
    }
}
