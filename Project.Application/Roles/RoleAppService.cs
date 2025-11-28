using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project.Application.Common.Dtos;
using Project.Core.Attributes;
using Project.Core.Constants;
using Project.Core.Entities;
using Project.Core.Interfaces;
using Project.Infrastructure.Extensions;
using Project.Application.Services;

using Project.Application.Roles.Dtos;
using Project.Core.Localization;

namespace Project.Application.Roles;

public class RoleAppService : AppServiceBase, IRoleAppService
{
    private readonly IRepository<Role> _roleRepository;
    private readonly IMapper _mapper;

    public RoleAppService(
        IRepository<Role> roleRepository,
        IMapper mapper,
        ICurrentUser currentUser,
        ICurrentTenant currentTenant,
        IPermissionChecker permissionChecker,
        ILocalizationManager localizationManager)
        : base(currentUser, currentTenant, permissionChecker, localizationManager)
    {
        _roleRepository = roleRepository;
        _mapper = mapper;
    }

    [RequiresPermission(PermissionNames.Pages_Roles)]
    public async Task<PagedResultDto<RoleDto>> GetListAsync(GetRolesInput input)
    {
        input.Normalize();

        var query = _roleRepository.GetQueryable();

        // Apply filter/search using WhereIf
        var filter = input.Filter?.ToLower();
        query = query.WhereIf(
            !string.IsNullOrWhiteSpace(filter),
            r => r.Name.ToLower().Contains(filter!) ||
                 (r.DisplayName != null && r.DisplayName.ToLower().Contains(filter!)));

        var totalCount = await query.CountAsync();

        var roles = await query
            .OrderByDynamic(input.Sorting ?? "Name ASC")
            .PageBy(input.SkipCount, input.MaxResultCount)
            .ToListAsync();

        return new PagedResultDto<RoleDto>
        {
            Items = _mapper.Map<List<RoleDto>>(roles),
            TotalCount = totalCount
        };
    }

    [RequiresPermission(PermissionNames.Pages_Roles)]
    public async Task<RoleDto> GetAsync(Guid id)
    {
        var role = await _roleRepository.GetAsync(id);
        if (role == null)
        {
            throw new Exception(await L("Common:NotFound", $"Role {id}"));
        }

        return _mapper.Map<RoleDto>(role);
    }

    [RequiresPermission(PermissionNames.Pages_Roles, PermissionNames.Pages_Roles_Create)]
    public async Task<RoleDto> CreateAsync(CreateRoleDto input)
    {
        var role = _mapper.Map<Role>(input);
        role.Id = Guid.NewGuid();
        
        // Ensure unique name
        var existingRole = await _roleRepository.GetQueryable()
            .FirstOrDefaultAsync(r => r.Name == input.Name);
        if (existingRole != null)
        {
            throw new Exception(await L("Common:AlreadyExists", $"Role {input.Name}"));
        }

        await _roleRepository.InsertAsync(role);

        return _mapper.Map<RoleDto>(role);
    }

    [RequiresPermission(PermissionNames.Pages_Roles, PermissionNames.Pages_Roles_Edit)]
    public async Task<RoleDto> UpdateAsync(Guid id, UpdateRoleDto input)
    {
        var role = await _roleRepository.GetAsync(id);
        if (role == null)
        {
            throw new Exception(await L("Common:NotFound", $"Role {id}"));
        }

        if (role.IsStatic)
        {
            throw new Exception(await L("Common:InvalidOperation"));
        }

        // Check if name is being changed and if it conflicts
        if (role.Name != input.Name)
        {
            var existingRole = await _roleRepository.GetQueryable()
                .FirstOrDefaultAsync(r => r.Name == input.Name);
            if (existingRole != null)
            {
                throw new Exception(await L("Common:AlreadyExists", $"Role {input.Name}"));
            }
        }

        _mapper.Map(input, role);
        await _roleRepository.UpdateAsync(role);

        return _mapper.Map<RoleDto>(role);
    }

    [RequiresPermission(PermissionNames.Pages_Roles, PermissionNames.Pages_Roles_Delete)]
    public async Task DeleteAsync(Guid id)
    {
        var role = await _roleRepository.GetAsync(id);
        if (role == null)
        {
            throw new Exception($"Role with id {id} not found");
        }

        if (role.IsStatic)
        {
            throw new Exception(await L("Common:InvalidOperation"));
        }

        await _roleRepository.DeleteAsync(role);
    }
}
