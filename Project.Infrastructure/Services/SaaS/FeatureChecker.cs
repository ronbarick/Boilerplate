using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Project.Domain.Entities.SaaS;
using Project.Domain.Interfaces;
using Project.Domain.Interfaces.DependencyInjection;
using Project.Domain.Services;

namespace Project.Infrastructure.Services.SaaS;

public class FeatureChecker : DomainService, IFeatureChecker, ITransientDependency
{
    private readonly IRepository<SaaSFeatureValue, Guid> _featureValueRepository;
    private readonly IRepository<SaasPlanFeature, Guid> _planFeatureRepository;
    private readonly IRepository<SaaSFeature, Guid> _featureRepository;
    private readonly IRepository<SaasTenantSubscription, Guid> _subscriptionRepository;
    private readonly ICurrentTenant _currentTenant;

    public FeatureChecker(
        IRepository<SaaSFeatureValue, Guid> featureValueRepository,
        IRepository<SaasPlanFeature, Guid> planFeatureRepository,
        IRepository<SaaSFeature, Guid> featureRepository,
        IRepository<SaasTenantSubscription, Guid> subscriptionRepository,
        ICurrentTenant currentTenant)
    {
        _featureValueRepository = featureValueRepository;
        _planFeatureRepository = planFeatureRepository;
        _featureRepository = featureRepository;
        _subscriptionRepository = subscriptionRepository;
        _currentTenant = currentTenant;
    }

    public async Task<bool> IsEnabledAsync(string featureName)
    {
        if (_currentTenant.Id == null) return false; 
        return await IsEnabledAsync(_currentTenant.Id.Value, featureName);
    }

    public async Task<bool> IsEnabledAsync(Guid tenantId, string featureName)
    {
        var value = await GetValueAsync(tenantId, featureName);
        return bool.TryParse(value, out var result) && result;
    }

    public async Task<string?> GetValueAsync(string featureName)
    {
        if (_currentTenant.Id == null) return null;
        return await GetValueAsync(_currentTenant.Id.Value, featureName);
    }

    public async Task<string?> GetValueAsync(Guid tenantId, string featureName)
    {
        // 1. Check Tenant Override
        var tenantValue = await _featureValueRepository.GetQueryable()
            .FirstOrDefaultAsync(fv => fv.TenantId == tenantId && fv.FeatureName == featureName);
        if (tenantValue != null) return tenantValue.FeatureValue;

        // 2. Check Plan Feature
        var subscription = await _subscriptionRepository.GetQueryable()
            .FirstOrDefaultAsync(s => s.TenantId == tenantId && s.IsCurrentSubscription);
        if (subscription != null)
        {
            var planFeature = await _planFeatureRepository.GetQueryable()
                .FirstOrDefaultAsync(pf => pf.PlanId == subscription.PlanId && pf.FeatureName == featureName);
            if (planFeature != null) return planFeature.FeatureValue;
        }

        // 3. Check Default Value
        var feature = await _featureRepository.GetQueryable()
            .FirstOrDefaultAsync(f => f.Name == featureName);
        return feature?.DefaultValue;
    }
}
