using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Project.Application.SaaS.Dtos;
using Project.Core.Interfaces;

namespace Project.Application.SaaS.Addons;

public interface IAddonAppService : IApplicationService
{
    Task<List<SaaSAddonDto>> GetListAsync();
    Task<SaaSAddonDto> GetAsync(Guid id);
    Task PurchaseAddonAsync(Guid tenantId, Guid addonId);
}
