using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Project.Core.Entities.SaaS;
using Project.Core.Enums;
using Project.Core.Interfaces;
using Project.Core.Interfaces.Common;
using Project.Core.Interfaces.DependencyInjection;
using Project.Core.Services;

namespace Project.Infrastructure.Services.SaaS;

public class FeatureManager : DomainService, IFeatureManager, ITransientDependency
{
    private readonly IRepository<SaaSFeature, Guid> _featureRepository;
    private readonly IRepository<SaaSFeatureValue, Guid> _featureValueRepository;
    private readonly IRepository<SaaSFeatureUsage, Guid> _usageRepository;
    private readonly IRepository<SaasPlanFeature, Guid> _planFeatureRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IClock _clock;

    public FeatureManager(
        IRepository<SaaSFeature, Guid> featureRepository,
        IRepository<SaaSFeatureValue, Guid> featureValueRepository,
        IRepository<SaaSFeatureUsage, Guid> usageRepository,
        IRepository<SaasPlanFeature, Guid> planFeatureRepository,
        IGuidGenerator guidGenerator,
        IClock clock)
    {
        _featureRepository = featureRepository;
        _featureValueRepository = featureValueRepository;
        _usageRepository = usageRepository;
        _planFeatureRepository = planFeatureRepository;
        _guidGenerator = guidGenerator;
        _clock = clock;
    }

    public async Task SetPlanFeatureAsync(Guid planId, string featureName, string value)
    {
        var planFeature = await _planFeatureRepository.GetQueryable()
            .FirstOrDefaultAsync(pf => pf.PlanId == planId && pf.FeatureName == featureName);
        
        if (planFeature == null)
        {
            var feature = await _featureRepository.GetQueryable()
                .FirstOrDefaultAsync(f => f.Name == featureName);
            if (feature == null) return; 

            planFeature = new SaasPlanFeature
            {
                Id = _guidGenerator.Create(),
                PlanId = planId,
                FeatureName = featureName,
                FeatureValue = value,
                FeatureType = feature.FeatureType
            };
            await _planFeatureRepository.InsertAsync(planFeature);
        }
        else
        {
            planFeature.FeatureValue = value;
            await _planFeatureRepository.UpdateAsync(planFeature);
        }
    }

    public async Task SetTenantFeatureAsync(Guid tenantId, string featureName, string value)
    {
        var featureValue = await _featureValueRepository.GetQueryable()
            .FirstOrDefaultAsync(fv => fv.TenantId == tenantId && fv.FeatureName == featureName);
        
        if (featureValue == null)
        {
            featureValue = new SaaSFeatureValue
            {
                Id = _guidGenerator.Create(),
                TenantId = tenantId,
                EntityId = tenantId,
                EntityType = FeatureEntityType.Tenant,
                FeatureName = featureName,
                FeatureValue = value
            };
            await _featureValueRepository.InsertAsync(featureValue);
        }
        else
        {
            featureValue.FeatureValue = value;
            await _featureValueRepository.UpdateAsync(featureValue);
        }
    }

    public async Task<string> GetFeatureValueAsync(Guid tenantId, string featureName)
    {
        var tenantValue = await _featureValueRepository.GetQueryable()
            .FirstOrDefaultAsync(fv => fv.TenantId == tenantId && fv.FeatureName == featureName);
        if (tenantValue != null) return tenantValue.FeatureValue;

        return null!; 
    }

    public async Task<bool> IsFeatureEnabledAsync(Guid tenantId, string featureName)
    {
        var val = await GetFeatureValueAsync(tenantId, featureName);
        return bool.TryParse(val, out var result) && result;
    }

    public async Task TrackFeatureUsageAsync(Guid tenantId, string featureName, int quantity = 1)
    {
        var currentMonth = new DateTime(_clock.Now.Year, _clock.Now.Month, 1);
        
        var usage = await _usageRepository.GetQueryable()
            .FirstOrDefaultAsync(u => u.FeatureName == featureName && u.UsageMonth == currentMonth); 
        
        if (usage == null)
        {
            usage = new SaaSFeatureUsage
            {
                Id = _guidGenerator.Create(),
                FeatureName = featureName,
                UsageCount = quantity,
                UsageMonth = currentMonth,
                AlertSent = false
            };
            await _usageRepository.InsertAsync(usage);
        }
        else
        {
            usage.UsageCount += quantity;
            await _usageRepository.UpdateAsync(usage);
        }
    }

    public async Task ResetFeatureUsageAsync(Guid tenantId, string featureName)
    {
        var currentMonth = new DateTime(_clock.Now.Year, _clock.Now.Month, 1);
        var usage = await _usageRepository.GetQueryable()
            .FirstOrDefaultAsync(u => u.FeatureName == featureName && u.UsageMonth == currentMonth);
        if (usage != null)
        {
            usage.UsageCount = 0;
            await _usageRepository.UpdateAsync(usage);
        }
    }

    public async Task CheckUsageAlertsAsync()
    {
        await Task.CompletedTask;
    }
}
