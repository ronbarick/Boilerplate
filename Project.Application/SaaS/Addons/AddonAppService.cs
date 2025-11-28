using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project.Application.SaaS.Dtos;
using Project.Application.Services;
using Project.Core.Entities.SaaS;
using Project.Core.Interfaces;
using Project.Core.Interfaces.Common;
using Project.Core.Localization;

namespace Project.Application.SaaS.Addons;

public class AddonAppService : AppServiceBase, IAddonAppService
{
    private readonly IRepository<SaaSAddon, Guid> _addonRepository;
    private readonly IRepository<SaasTenantAddon, Guid> _tenantAddonRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IMapper _mapper;

    public AddonAppService(
        IRepository<SaaSAddon, Guid> addonRepository,
        IRepository<SaasTenantAddon, Guid> tenantAddonRepository,
        IGuidGenerator guidGenerator,
        IMapper mapper,
        ICurrentUser currentUser,
        ICurrentTenant currentTenant,
        IPermissionChecker permissionChecker,
        ILocalizationManager localizationManager)
        : base(currentUser, currentTenant, permissionChecker, localizationManager)
    {
        _addonRepository = addonRepository;
        _tenantAddonRepository = tenantAddonRepository;
        _guidGenerator = guidGenerator;
        _mapper = mapper;
    }

    public async Task<List<SaaSAddonDto>> GetListAsync()
    {
        var addons = await _addonRepository.GetQueryable().ToListAsync();
        return _mapper.Map<List<SaaSAddonDto>>(addons);
    }

    public async Task<SaaSAddonDto> GetAsync(Guid id)
    {
        var addon = await _addonRepository.GetAsync(id);
        return _mapper.Map<SaaSAddonDto>(addon);
    }

    public async Task PurchaseAddonAsync(Guid tenantId, Guid addonId)
    {
        var tenantAddon = new SaasTenantAddon
        {
            Id = _guidGenerator.Create(),
            TenantId = tenantId,
            AddonId = addonId,
            PurchaseDate = DateTime.UtcNow,
            IsActive = true
        };
        await _tenantAddonRepository.InsertAsync(tenantAddon);
    }
}
