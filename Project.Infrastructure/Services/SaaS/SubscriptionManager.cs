using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Project.Domain.Shared.Constants;
using Project.Domain.Entities.SaaS;
using Project.Domain.Shared.Enums;
using Project.Domain.Shared.Exceptions;
using Project.Domain.Interfaces;
using Project.Domain.Interfaces.Common;
using Project.Domain.Interfaces.DependencyInjection;
using Project.Domain.Services;
// using Volo.Abp.Domain.Repositories; // Removed

namespace Project.Infrastructure.Services.SaaS;

public class SubscriptionManager : DomainService, ISubscriptionManager, ITransientDependency
{
    private readonly IRepository<SaasTenantSubscription, Guid> _subscriptionRepository;
    private readonly IRepository<SaasPlan, Guid> _planRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IClock _clock;
    private readonly ITenantSyncService _tenantSyncService;

    public SubscriptionManager(
        IRepository<SaasTenantSubscription, Guid> subscriptionRepository,
        IRepository<SaasPlan, Guid> planRepository,
        IGuidGenerator guidGenerator,
        IClock clock,
        ITenantSyncService tenantSyncService)
    {
        _subscriptionRepository = subscriptionRepository;
        _planRepository = planRepository;
        _guidGenerator = guidGenerator;
        _clock = clock;
        _tenantSyncService = tenantSyncService;
    }

    public async Task<SaasTenantSubscription> CreateSubscriptionAsync(Guid tenantId, Guid planId, BillingCycle billingCycle)
    {
        var plan = await _planRepository.GetAsync(planId);
        if (plan == null) throw new UserFriendlyException("Plan not found");
        
        // Deactivate existing current subscriptions
        var existingSubscriptions = await _subscriptionRepository.GetQueryable()
            .Where(s => s.TenantId == tenantId && s.IsCurrentSubscription)
            .ToListAsync();
            
        foreach (var sub in existingSubscriptions)
        {
            sub.IsCurrentSubscription = false;
            await _subscriptionRepository.UpdateAsync(sub);
        }

        var startDate = _clock.Now;
        DateTime? endDate = null;
        DateTime? trialEndDate = null;

        if (plan.TrialDays > 0)
        {
            trialEndDate = startDate.AddDays(plan.TrialDays);
            endDate = trialEndDate; 
        }
        else
        {
            endDate = billingCycle == BillingCycle.Monthly ? startDate.AddMonths(1) : startDate.AddYears(1);
        }

        var subscription = new SaasTenantSubscription
        {
            Id = _guidGenerator.Create(),
            TenantId = tenantId,
            PlanId = planId,
            Status = plan.TrialDays > 0 ? SubscriptionStatus.Trial : SubscriptionStatus.Active,
            IsCurrentSubscription = true,
            StartDate = startDate,
            EndDate = endDate,
            TrialEndDate = trialEndDate,
            AutoRenew = true,
            GracePeriodDays = int.Parse(SaaSSettingNames.GracePeriodDays) 
        };

        await _subscriptionRepository.InsertAsync(subscription);
        await _tenantSyncService.SyncSubscriptionAsync(subscription);

        return subscription;
    }

    public async Task<SaasTenantSubscription> UpgradeSubscriptionAsync(Guid tenantId, Guid newPlanId)
    {
        var currentSub = await GetCurrentSubscriptionAsync(tenantId);
        if (currentSub == null)
        {
            throw new UserFriendlyException("No active subscription found to upgrade.");
        }

        var newPlan = await _planRepository.GetAsync(newPlanId);
        if (newPlan == null) throw new UserFriendlyException("New plan not found");
        
        currentSub.IsCurrentSubscription = false;
        currentSub.EndDate = _clock.Now; 
        await _subscriptionRepository.UpdateAsync(currentSub);

        var newSub = new SaasTenantSubscription
        {
            Id = _guidGenerator.Create(),
            TenantId = tenantId,
            PlanId = newPlanId,
            Status = SubscriptionStatus.Active,
            IsCurrentSubscription = true,
            StartDate = _clock.Now,
            EndDate = newPlan.BillingCycle == BillingCycle.Monthly ? _clock.Now.AddMonths(1) : _clock.Now.AddYears(1),
            AutoRenew = true
        };

        await _subscriptionRepository.InsertAsync(newSub);
        await _tenantSyncService.SyncSubscriptionAsync(newSub);

        return newSub;
    }

    public async Task<SaasTenantSubscription> DowngradeSubscriptionAsync(Guid tenantId, Guid newPlanId)
    {
        var currentSub = await GetCurrentSubscriptionAsync(tenantId);
        if (currentSub == null)
        {
            throw new UserFriendlyException("No active subscription found.");
        }

        currentSub.IsCurrentSubscription = false;
        currentSub.EndDate = _clock.Now;
        await _subscriptionRepository.UpdateAsync(currentSub);

        var newPlan = await _planRepository.GetAsync(newPlanId);
        if (newPlan == null) throw new UserFriendlyException("New plan not found");

        var newSub = new SaasTenantSubscription
        {
            Id = _guidGenerator.Create(),
            TenantId = tenantId,
            PlanId = newPlanId,
            Status = SubscriptionStatus.Active,
            IsCurrentSubscription = true,
            StartDate = _clock.Now,
            EndDate = newPlan.BillingCycle == BillingCycle.Monthly ? _clock.Now.AddMonths(1) : _clock.Now.AddYears(1),
            AutoRenew = true
        };

        await _subscriptionRepository.InsertAsync(newSub);
        await _tenantSyncService.SyncSubscriptionAsync(newSub);

        return newSub;
    }

    public async Task CancelSubscriptionAsync(Guid tenantId, string reason, CancellationType type)
    {
        var sub = await GetCurrentSubscriptionAsync(tenantId);
        if (sub == null) return;

        sub.CancellationReason = reason;
        sub.CancellationType = type;
        sub.CancellationDate = _clock.Now;
        sub.AutoRenew = false;

        if (type == CancellationType.Immediate)
        {
            sub.Status = SubscriptionStatus.Cancelled;
            sub.EndDate = _clock.Now;
        }

        await _subscriptionRepository.UpdateAsync(sub);
        await _tenantSyncService.SyncSubscriptionAsync(sub);
    }

    public async Task PauseSubscriptionAsync(Guid tenantId, string reason)
    {
        var sub = await GetCurrentSubscriptionAsync(tenantId);
        if (sub == null) return;

        if (sub.Status != SubscriptionStatus.Active && sub.Status != SubscriptionStatus.Trial)
        {
            throw new UserFriendlyException("Only active or trial subscriptions can be paused.");
        }

        sub.Status = SubscriptionStatus.Paused;
        sub.PausedDate = _clock.Now;
        sub.PauseReason = reason;

        await _subscriptionRepository.UpdateAsync(sub);
    }

    public async Task ResumeSubscriptionAsync(Guid tenantId)
    {
        var sub = await GetCurrentSubscriptionAsync(tenantId);
        if (sub == null) return;

        if (sub.Status != SubscriptionStatus.Paused)
        {
            throw new UserFriendlyException("Subscription is not paused.");
        }

        if (sub.PausedDate.HasValue && sub.EndDate.HasValue)
        {
            var pausedDuration = _clock.Now - sub.PausedDate.Value;
            sub.EndDate = sub.EndDate.Value.Add(pausedDuration);
        }

        sub.Status = SubscriptionStatus.Active; 
        sub.PausedDate = null;
        sub.PauseReason = null;

        await _subscriptionRepository.UpdateAsync(sub);
    }

    public async Task ExtendTrialAsync(Guid tenantId, int days)
    {
        var sub = await GetCurrentSubscriptionAsync(tenantId);
        if (sub == null || sub.Status != SubscriptionStatus.Trial)
        {
            throw new UserFriendlyException("No active trial subscription found.");
        }

        if (sub.TrialEndDate.HasValue)
        {
            sub.TrialEndDate = sub.TrialEndDate.Value.AddDays(days);
            sub.EndDate = sub.TrialEndDate; 
            sub.TrialExtensionCount++;
            
            sub.TrialExtensionHistory = (sub.TrialExtensionHistory ?? "") + $"|Extended {days} days on {_clock.Now}";
            
            await _subscriptionRepository.UpdateAsync(sub);
            await _tenantSyncService.SyncSubscriptionAsync(sub);
        }
    }

    public async Task<bool> IsSubscriptionActiveAsync(Guid tenantId)
    {
        var sub = await GetCurrentSubscriptionAsync(tenantId);
        if (sub == null) return false;

        return sub.Status == SubscriptionStatus.Active || sub.Status == SubscriptionStatus.Trial;
    }

    public async Task CheckExpirationsAsync()
    {
        var expiredSubs = await _subscriptionRepository.GetQueryable()
            .Where(s => (s.Status == SubscriptionStatus.Active || s.Status == SubscriptionStatus.Trial) && s.EndDate < _clock.Now)
            .ToListAsync();

        foreach (var sub in expiredSubs)
        {
            if (sub.AutoRenew)
            {
                // Logic to renew
            }
            else
            {
                sub.Status = SubscriptionStatus.Expired;
                await _subscriptionRepository.UpdateAsync(sub);
                await _tenantSyncService.SyncSubscriptionAsync(sub);
            }
        }
    }

    private async Task<SaasTenantSubscription?> GetCurrentSubscriptionAsync(Guid tenantId)
    {
        return await _subscriptionRepository.GetQueryable()
            .FirstOrDefaultAsync(s => s.TenantId == tenantId && s.IsCurrentSubscription);
    }
}
