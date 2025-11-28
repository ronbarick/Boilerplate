using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project.Application.SaaS.Dtos;
using Project.Application.Services;
using Project.Core.Entities.SaaS;
using Project.Core.Interfaces;
using Project.Core.Localization;

namespace Project.Application.SaaS.Features;

public class FeatureAppService : AppServiceBase, IFeatureAppService
{
    private readonly IRepository<SaaSFeature, Guid> _featureRepository;
    private readonly IFeatureManager _featureManager;
    private readonly IFeatureChecker _featureChecker;
    private readonly IMapper _mapper;

    public FeatureAppService(
        IRepository<SaaSFeature, Guid> featureRepository,
        IFeatureManager featureManager,
        IFeatureChecker featureChecker,
        IMapper mapper,
        ICurrentUser currentUser,
        ICurrentTenant currentTenant,
        IPermissionChecker permissionChecker,
        ILocalizationManager localizationManager)
        : base(currentUser, currentTenant, permissionChecker, localizationManager)
    {
        _featureRepository = featureRepository;
        _featureManager = featureManager;
        _featureChecker = featureChecker;
        _mapper = mapper;
    }

    public async Task<List<SaaSFeatureDto>> GetListAsync()
    {
        var features = await _featureRepository.GetQueryable().ToListAsync();
        return _mapper.Map<List<SaaSFeatureDto>>(features);
    }

    public async Task<string> GetFeatureValueAsync(Guid tenantId, string featureName)
    {
        return await _featureChecker.GetValueAsync(tenantId, featureName);
    }

    public async Task<bool> IsFeatureEnabledAsync(Guid tenantId, string featureName)
    {
        return await _featureChecker.IsEnabledAsync(tenantId, featureName);
    }

    public async Task IncrementFeatureUsageAsync(Guid tenantId, string featureName, int incrementBy = 1)
    {
        await _featureManager.TrackFeatureUsageAsync(tenantId, featureName, incrementBy);
    }
}
