using System;
using System.Threading.Tasks;
using Project.Application.Common.Dtos;
using Project.Application.Tenants.Dtos;
using Project.Application.Services;
using Project.Domain.Interfaces;

namespace Project.Application.Tenants;

public interface ITenantAppService : IApplicationService
{
    Task<PagedResultDto<TenantDto>> GetListAsync(GetTenantsInput input);
    Task<TenantDto> CreateAsync(CreateTenantDto input);
    Task<IsTenantAvailableOutput> IsTenantAvailableAsync(IsTenantAvailableInput input);
    Task<TenantDto?> GetByNameAsync(string name);
}
