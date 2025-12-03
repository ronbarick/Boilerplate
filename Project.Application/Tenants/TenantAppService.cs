using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project.Application.Common.Dtos;
using Project.Domain.Attributes;

using Project.Domain.Entities;
using Project.Domain.Interfaces;
using Project.Infrastructure.Extensions;
using Project.Infrastructure.Services;
using Project.Application.Services;

using Project.Application.Tenants.Dtos;
using Project.Domain.Localization;
using Microsoft.AspNetCore.Authorization;

namespace Project.Application.Tenants;

public class TenantAppService : AppServiceBase, ITenantAppService
{
    private readonly IRepository<Tenant> _tenantRepository;
    private readonly IMapper _mapper;
    private readonly TenantProvisioningService _provisioningService;

    public TenantAppService(
        IRepository<Tenant> tenantRepository,
        IMapper mapper,
        ICurrentUser currentUser,
        ICurrentTenant currentTenant,
        IPermissionChecker permissionChecker,
        TenantProvisioningService provisioningService,
        ILocalizationManager localizationManager)
        : base(currentUser, currentTenant, permissionChecker, localizationManager)
    {
        _tenantRepository = tenantRepository;
        _mapper = mapper;
        _provisioningService = provisioningService;
    }

    [RequiresPermission(PermissionNames.Pages_Tenants)]
    public async Task<PagedResultDto<TenantDto>> GetListAsync(GetTenantsInput input)
    {
        input.Normalize();

        // Get all tenants (ignore tenant filter for host admin)
        var query = _tenantRepository
            .GetQueryable()

            .Where(t => !t.IsDeleted);

        // Apply filter/search using WhereIf
        var filter = input.Filter?.ToLower();
        query = query.WhereIf(
            !string.IsNullOrWhiteSpace(filter),
            t => t.Name.ToLower().Contains(filter!) ||
                 t.TenancyName.ToLower().Contains(filter!));

        var totalCount = await query.CountAsync();

        var tenants = await query
            .OrderByDynamic(input.Sorting ?? "Name ASC")
            .PageBy(input.SkipCount, input.MaxResultCount)
            .ToListAsync();

        return new PagedResultDto<TenantDto>
        {
            Items = _mapper.Map<List<TenantDto>>(tenants),
            TotalCount = totalCount
        };
    }

    [RequiresPermission(PermissionNames.Pages_Tenants, PermissionNames.Pages_Tenants_Create)]
    public async Task<TenantDto> CreateAsync(CreateTenantDto input)
    {
        var tenant = _mapper.Map<Tenant>(input);
        tenant.Id = Guid.NewGuid();
        tenant.IsActive = true;
        tenant.CreatedOn = DateTime.UtcNow;

        await _tenantRepository.InsertAsync(tenant, autoSave: true);

        // Provision tenant database
        await _provisioningService.ProvisionTenantAsync(tenant, input.AdminPassword);

        // Update tenant with connection string (if modified by provisioning)
        await _tenantRepository.UpdateAsync(tenant, autoSave: true);

        return _mapper.Map<TenantDto>(tenant);
    }

    [AllowAnonymous]
    public async Task<IsTenantAvailableOutput> IsTenantAvailableAsync(IsTenantAvailableInput input)
    {
        if (string.IsNullOrWhiteSpace(input.TenancyName))
        {
            return new IsTenantAvailableOutput
            {
                State = TenantAvailabilityState.NotFound
            };
        }

        var tenant = await _tenantRepository.GetQueryable()
            .IgnoreQueryFilters() // Allow checking all tenants, even if current context is tenant-scoped
            .FirstOrDefaultAsync(t => t.TenancyName == input.TenancyName && !t.IsDeleted);

        if (tenant == null)
        {
            return new IsTenantAvailableOutput
            {
                State = TenantAvailabilityState.NotFound
            };
        }

        if (!tenant.IsActive)
        {
            return new IsTenantAvailableOutput
            {
                State = TenantAvailabilityState.InActive,
                TenantId = tenant.Id,
                Name = tenant.Name
            };
        }

        return new IsTenantAvailableOutput
        {
            State = TenantAvailabilityState.Available,
            TenantId = tenant.Id,
            Name = tenant.Name
        };
    }

    [AllowAnonymous]
    public async Task<TenantDto?> GetByNameAsync(string name)
    {
        var tenant = await _tenantRepository.GetQueryable()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.TenancyName == name && !t.IsDeleted && t.IsActive);

        if (tenant == null)
            return null;

        return _mapper.Map<TenantDto>(tenant);
    }
}
