using System;
using System.Threading.Tasks;
using Project.Application.SaaS.Dtos;
using Project.Domain.Interfaces;

namespace Project.Application.SaaS.Analytics;

public interface IAnalyticsAppService : IApplicationService
{
    Task<SaaSAnalyticsDto> GetAnalyticsAsync(Guid? tenantId = null);
}
