using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Project.Application.Common.Dtos;
using Project.Domain.Attributes;

using Project.Domain.Entities;
using Project.Domain.Interfaces;
using Project.Application.Services;

using Project.Application.Permissions.Dtos;
using Project.Domain.Localization;

namespace Project.Application.Permissions;

public class PermissionAppService : AppServiceBase, IPermissionAppService
{
    private readonly IPermissionManager _permissionManager;
    private readonly IRepository<PermissionDefinition> _permissionDefinitionRepository;
    private readonly IRepository<Role> _roleRepository;
    private readonly IRepository<UserPermission> _userPermissionRepository;
    private readonly IRepository<UserRole> _userRoleRepository;
    private readonly IRepository<RolePermission> _rolePermissionRepository;

    public PermissionAppService(
        ICurrentUser currentUser,
        ICurrentTenant currentTenant,
        IPermissionChecker permissionChecker,
        IPermissionManager permissionManager,
        IRepository<PermissionDefinition> permissionDefinitionRepository,
        IRepository<Role> roleRepository,
        IRepository<UserPermission> userPermissionRepository,
        IRepository<UserRole> userRoleRepository,
        IRepository<RolePermission> rolePermissionRepository,
        ILocalizationManager localizationManager)
        : base(currentUser, currentTenant, permissionChecker, localizationManager)
    {
        _permissionManager = permissionManager;
        _permissionDefinitionRepository = permissionDefinitionRepository;
        _roleRepository = roleRepository;
        _userPermissionRepository = userPermissionRepository;
        _userRoleRepository = userRoleRepository;
        _rolePermissionRepository = rolePermissionRepository;
    }

    [RequiresPermission(PermissionNames.Pages_Roles)]
    public async Task<GetPermissionListResultDto> GetAllPermissionsAsync()
    {
        var permissions = await _permissionDefinitionRepository
            .GetQueryable()
            
            .ToListAsync();

        var result = new GetPermissionListResultDto
        {
            EntityDisplayName = "Permissions",
            Groups = new List<PermissionGroupDto>()
        };

        // Group by root permissions (those without parent) or by logical groups if we had them
        // For now, let's put everything in a "Main" group or group by top-level parents
        
        var rootPermissions = permissions.Where(p => string.IsNullOrEmpty(p.ParentName)).ToList();
        var mainGroup = new PermissionGroupDto
        {
            Name = "Main",
            DisplayName = "All Permissions",
            Permissions = new List<PermissionDto>()
        };

        foreach (var rootPerm in rootPermissions)
        {
            mainGroup.Permissions.Add(BuildPermissionTree(rootPerm, permissions));
        }

        result.Groups.Add(mainGroup);
        return result;
    }

    [RequiresPermission(PermissionNames.Pages_Roles)]
    public async Task<GetPermissionListResultDto> GetRolePermissionsAsync(GetRolePermissionsInput input)
    {
        // Resolve Role ID
        Guid roleId;
        if (input.RoleId.HasValue)
        {
            roleId = input.RoleId.Value;
        }
        else if (!string.IsNullOrEmpty(input.RoleName))
        {
            var role = await _roleRepository.GetQueryable().FirstOrDefaultAsync(r => r.Name == input.RoleName);
            if (role == null) throw new ArgumentException(await L("Common:NotFound", $"Role {input.RoleName}"));
            roleId = role.Id;
        }
        else
        {
            throw new ArgumentException(await L("Permission:RoleRequired"));
        }

        // Get all definitions
        var allPermissions = await _permissionDefinitionRepository
            .GetQueryable()
            
            .ToListAsync();

        // Get granted permissions for role
        var grantedPermissions = await _permissionManager.GetForRoleAsync(roleId);

        var result = new GetPermissionListResultDto
        {
            EntityDisplayName = "Role Permissions",
            Groups = new List<PermissionGroupDto>()
        };

        var mainGroup = new PermissionGroupDto
        {
            Name = "Main",
            DisplayName = "Permissions",
            Permissions = new List<PermissionDto>()
        };

        var rootPermissions = allPermissions.Where(p => string.IsNullOrEmpty(p.ParentName)).ToList();
        foreach (var rootPerm in rootPermissions)
        {
            mainGroup.Permissions.Add(BuildPermissionTree(rootPerm, allPermissions, grantedPermissions));
        }

        result.Groups.Add(mainGroup);
        return result;
    }

    [RequiresPermission(PermissionNames.Pages_Roles_Edit)]
    public async Task SetRolePermissionsAsync(UpdateRolePermissionsInput input)
    {
        // Resolve Role ID
        Guid roleId;
        if (input.RoleId.HasValue)
        {
            roleId = input.RoleId.Value;
        }
        else if (!string.IsNullOrEmpty(input.RoleName))
        {
            var role = await _roleRepository.GetQueryable().FirstOrDefaultAsync(r => r.Name == input.RoleName);
            if (role == null) throw new ArgumentException($"Role {input.RoleName} not found");
            roleId = role.Id;
        }
        else
        {
            throw new ArgumentException("RoleId or RoleName must be provided");
        }

        foreach (var perm in input.Permissions)
        {
            await _permissionManager.SetForRoleAsync(roleId, perm.Name, perm.IsGranted);
        }
    }

    [RequiresPermission(PermissionNames.Pages_Users_ChangePermissions)]
    public async Task<GetPermissionListResultDto> GetUserPermissionsAsync(Guid userId)
    {
        // Get all definitions
        var allPermissions = await _permissionDefinitionRepository
            .GetQueryable()
            
            .ToListAsync();

        // Get user explicit permissions
        var userExplicitPermissions = await _userPermissionRepository
            .GetQueryable()
            
            .Where(up => up.UserId == userId)
            .ToDictionaryAsync(up => up.PermissionName, up => up.IsGranted);

        // Get user roles
        var userRoles = await _userRoleRepository
            .GetQueryable()
            
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync();

        // Get role permissions
        var rolePermissions = await _rolePermissionRepository
            .GetQueryable()
            
            .Where(rp => userRoles.Contains(rp.RoleId) && rp.IsGranted)
            .Select(rp => rp.PermissionName)
            .Distinct()
            .ToListAsync();
        var roleGrantedSet = new HashSet<string>(rolePermissions);

        var result = new GetPermissionListResultDto
        {
            EntityDisplayName = "User Permissions",
            Groups = new List<PermissionGroupDto>()
        };

        var mainGroup = new PermissionGroupDto
        {
            Name = "Main",
            DisplayName = "Permissions",
            Permissions = new List<PermissionDto>()
        };

        var rootPermissions = allPermissions.Where(p => string.IsNullOrEmpty(p.ParentName)).ToList();
        foreach (var rootPerm in rootPermissions)
        {
            mainGroup.Permissions.Add(BuildUserPermissionTree(rootPerm, allPermissions, userExplicitPermissions, roleGrantedSet));
        }

        result.Groups.Add(mainGroup);
        return result;
    }

    [RequiresPermission(PermissionNames.Pages_Users_ChangePermissions)]
    public async Task SetUserPermissionsAsync(Guid userId, UpdatePermissionsDto input)
    {
        foreach (var perm in input.Permissions)
        {
            await _permissionManager.SetForUserAsync(userId, perm.Name, perm.IsGranted);
        }
    }

    // Helper Methods

    private PermissionDto BuildPermissionTree(
        PermissionDefinition def, 
        List<PermissionDefinition> allDefs, 
        Dictionary<string, bool>? grantedPermissions = null)
    {
        var dto = new PermissionDto
        {
            Name = def.Name,
            DisplayName = def.DisplayName,
            ParentName = def.ParentName,
            IsGranted = grantedPermissions != null && grantedPermissions.ContainsKey(def.Name) && grantedPermissions[def.Name],
            GrantState = grantedPermissions != null && grantedPermissions.ContainsKey(def.Name) && grantedPermissions[def.Name] 
                ? PermissionGrantState.Granted 
                : PermissionGrantState.NotGranted
        };

        var children = allDefs.Where(p => p.ParentName == def.Name).ToList();
        foreach (var child in children)
        {
            dto.Children.Add(BuildPermissionTree(child, allDefs, grantedPermissions));
        }

        return dto;
    }

    private PermissionDto BuildUserPermissionTree(
        PermissionDefinition def,
        List<PermissionDefinition> allDefs,
        Dictionary<string, bool> userExplicitPermissions,
        HashSet<string> roleGrantedPermissions)
    {
        var dto = new PermissionDto
        {
            Name = def.Name,
            DisplayName = def.DisplayName,
            ParentName = def.ParentName
        };

        // Determine Grant State
        if (userExplicitPermissions.ContainsKey(def.Name))
        {
            // Explicitly set on user
            dto.IsGranted = userExplicitPermissions[def.Name];
            dto.GrantState = dto.IsGranted ? PermissionGrantState.Granted : PermissionGrantState.Revoked;
        }
        else if (roleGrantedPermissions.Contains(def.Name))
        {
            // Inherited from role
            dto.IsGranted = true;
            dto.GrantState = PermissionGrantState.Inherited;
        }
        else
        {
            // Not granted
            dto.IsGranted = false;
            dto.GrantState = PermissionGrantState.NotGranted;
        }

        var children = allDefs.Where(p => p.ParentName == def.Name).ToList();
        foreach (var child in children)
        {
            dto.Children.Add(BuildUserPermissionTree(child, allDefs, userExplicitPermissions, roleGrantedPermissions));
        }

        return dto;
    }
}
