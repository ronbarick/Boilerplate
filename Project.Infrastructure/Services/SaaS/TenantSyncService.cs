using System;
using System.Threading.Tasks;
using Project.Domain.Entities;
using Project.Domain.Entities.SaaS;
using Project.Domain.Interfaces;
using Project.Domain.Interfaces.DependencyInjection;
using Project.Domain.Services;

namespace Project.Infrastructure.Services.SaaS;

public class TenantSyncService : DomainService, ITenantSyncService, ITransientDependency
{
    private readonly IRepository<Tenant, Guid> _tenantRepository;

    public TenantSyncService(IRepository<Tenant, Guid> tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task SyncSubscriptionAsync(SaasTenantSubscription subscription)
    {
        var tenant = await _tenantRepository.GetAsync(subscription.TenantId);
        if (tenant == null) return;

        // Update legacy fields for backward compatibility
        // tenant.EditionId = subscription.PlanId; // Incompatible types (Guid vs long)
        tenant.EditionName = subscription.Plan?.Name; 
        
        if (subscription.Status == Project.Domain.Shared.Enums.SubscriptionStatus.Trial)
        {
            tenant.IsInTrialPeriod = true;
            tenant.SubscriptionEndDate = subscription.TrialEndDate;
        }
        else if (subscription.Status == Project.Domain.Shared.Enums.SubscriptionStatus.Active)
        {
            tenant.IsInTrialPeriod = false;
            tenant.SubscriptionEndDate = subscription.EndDate;
        }
        else
        {
            tenant.IsInTrialPeriod = false;
            // For expired/cancelled, we might keep the last end date or set to null/past
            tenant.SubscriptionEndDate = subscription.EndDate; 
        }

        await _tenantRepository.UpdateAsync(tenant);
    }

    public async Task SyncFeatureAsync(Guid tenantId, string featureName, string value)
    {
        // If Tenant entity has specific fields for features, update them here
        // For now, we assume features are only in SaaS tables, but this hook allows future sync
        await Task.CompletedTask;
    }

    public async Task SyncTenantDetailsAsync(Guid tenantId)
    {
        // Sync other details if needed
        await Task.CompletedTask;
    }
}
