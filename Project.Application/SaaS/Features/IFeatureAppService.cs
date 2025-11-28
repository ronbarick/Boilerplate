using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Project.Application.SaaS.Dtos;
using Project.Core.Interfaces;

namespace Project.Application.SaaS.Features;

public interface IFeatureAppService : IApplicationService
{
    Task<List<SaaSFeatureDto>> GetListAsync();
    Task<string> GetFeatureValueAsync(Guid tenantId, string featureName);
    Task<bool> IsFeatureEnabledAsync(Guid tenantId, string featureName);
    Task IncrementFeatureUsageAsync(Guid tenantId, string featureName, int incrementBy = 1);
}
