using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Project.Domain.Entities;
using Project.Domain.Interfaces;
using Project.Domain.Localization;

namespace Project.Infrastructure.Services;

public class PermissionManager : IPermissionManager
{
    private readonly IRepository<RolePermission> _rolePermissionRepository;
    private readonly IRepository<Role> _roleRepository;
    private readonly IRepository<UserPermission> _userPermissionRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<UserRole> _userRoleRepository;
    private readonly ILocalizationManager _localizationManager;

    public PermissionManager(
        IRepository<RolePermission> rolePermissionRepository,
        IRepository<Role> roleRepository,
        IRepository<UserPermission> userPermissionRepository,
        IRepository<User> userRepository,
        IRepository<UserRole> userRoleRepository,
        ILocalizationManager localizationManager)
    {
        _rolePermissionRepository = rolePermissionRepository;
        _roleRepository = roleRepository;
        _userPermissionRepository = userPermissionRepository;
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _localizationManager = localizationManager;
    }

    // Role Permission Methods

    public async Task<Dictionary<string, bool>> GetForRoleAsync(Guid roleId)
    {
        return await _rolePermissionRepository
            .GetQueryable()
            .Where(rp => rp.RoleId == roleId)
            .ToDictionaryAsync(rp => rp.PermissionName, rp => rp.IsGranted);
    }

    public async Task<Dictionary<string, bool>> GetForRoleAsync(string roleName)
    {
        var role = await _roleRepository
            .GetQueryable()
            .FirstOrDefaultAsync(r => r.Name == roleName);

        if (role == null)
        {
            throw new ArgumentException(await _localizationManager.GetStringAsync(Project.Domain.Localization.ProjectLocalizationResource.ResourceName, "Common:NotFound", $"Role {roleName}"));
        }

        return await GetForRoleAsync(role.Id);
    }

    public async Task SetForRoleAsync(Guid roleId, string permissionName, bool isGranted)
    {
        var rolePermission = await _rolePermissionRepository
            .GetQueryable()
            .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionName == permissionName);

        if (rolePermission == null)
        {
            if (isGranted)
            {
                // Only add if granted (default is not granted if not present)
                rolePermission = new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleId = roleId,
                    PermissionName = permissionName,
                    IsGranted = true,
                    CreatedOn = DateTime.UtcNow
                };
                
                // Need to set TenantId if role has one
                var role = await _roleRepository.GetQueryable().FirstOrDefaultAsync(r => r.Id == roleId);
                if (role != null)
                {
                    rolePermission.TenantId = role.TenantId;
                }
                
                await _rolePermissionRepository.InsertAsync(rolePermission);
            }
        }
        else
        {
            if (isGranted)
            {
                rolePermission.IsGranted = true;
                await _rolePermissionRepository.UpdateAsync(rolePermission);
            }
            else
            {
                // If revoking, we can just remove the record since absence means not granted for roles
                await _rolePermissionRepository.DeleteAsync(rolePermission);
            }
        }
    }

    public async Task GrantAllAsync(Guid roleId, string[] permissionNames)
    {
        var role = await _roleRepository.GetQueryable().FirstOrDefaultAsync(r => r.Id == roleId);
        if (role == null) throw new ArgumentException(await _localizationManager.GetStringAsync(Project.Domain.Localization.ProjectLocalizationResource.ResourceName, "Common:NotFound", $"Role {roleId}"));

        var existingPermissions = await _rolePermissionRepository
            .GetQueryable()
            .Where(rp => rp.RoleId == roleId)
            .ToListAsync();

        foreach (var permissionName in permissionNames)
        {
            var existing = existingPermissions.FirstOrDefault(p => p.PermissionName == permissionName);
            if (existing == null)
            {
                await _rolePermissionRepository.InsertAsync(new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleId = roleId,
                    PermissionName = permissionName,
                    IsGranted = true,
                    TenantId = role.TenantId,
                    CreatedOn = DateTime.UtcNow
                });
            }
            else if (!existing.IsGranted)
            {
                existing.IsGranted = true;
                await _rolePermissionRepository.UpdateAsync(existing);
            }
        }
    }

    public async Task SetForRoleAsync(string roleName, string permissionName, bool isGranted)
    {
        var role = await _roleRepository
            .GetQueryable()
            .FirstOrDefaultAsync(r => r.Name == roleName);

        if (role == null)
        {
            throw new ArgumentException(await _localizationManager.GetStringAsync(Project.Domain.Localization.ProjectLocalizationResource.ResourceName, "Common:NotFound", $"Role {roleName}"));
        }

        await SetForRoleAsync(role.Id, permissionName, isGranted);
    }

    public async Task GrantAllAsync(string roleName, string[] permissionNames)
    {
        var role = await _roleRepository
            .GetQueryable()
            .FirstOrDefaultAsync(r => r.Name == roleName);

        if (role == null)
        {
            throw new ArgumentException(await _localizationManager.GetStringAsync(Project.Domain.Localization.ProjectLocalizationResource.ResourceName, "Common:NotFound", $"Role {roleName}"));
        }

        await GrantAllAsync(role.Id, permissionNames);
    }

    // User Permission Methods

    public async Task<Dictionary<string, bool>> GetForUserAsync(Guid userId)
    {
        // 1. Get user-specific permissions (overrides)
        var userPermissions = await _userPermissionRepository
            .GetQueryable()
            .Where(up => up.UserId == userId)
            .ToDictionaryAsync(up => up.PermissionName, up => up.IsGranted);

        // 2. Get user roles
        var userRoles = await _userRoleRepository
            .GetQueryable()
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync();

        // 3. Get role permissions
        var rolePermissions = await _rolePermissionRepository
            .GetQueryable()
            .Where(rp => userRoles.Contains(rp.RoleId) && rp.IsGranted)
            .Select(rp => rp.PermissionName)
            .Distinct()
            .ToListAsync();

        // Merge: User permissions override role permissions
        var result = new Dictionary<string, bool>();

        // Add role permissions (default granted)
        foreach (var perm in rolePermissions)
        {
            result[perm] = true;
        }

        // Apply user overrides
        foreach (var kvp in userPermissions)
        {
            result[kvp.Key] = kvp.Value;
        }

        return result;
    }

    public async Task SetForUserAsync(Guid userId, string permissionName, bool isGranted)
    {
        var userPermission = await _userPermissionRepository
            .GetQueryable()
            .FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionName == permissionName);

        if (userPermission == null)
        {
            userPermission = new UserPermission
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PermissionName = permissionName,
                IsGranted = isGranted,
                CreatedOn = DateTime.UtcNow
            };
            
            // Need to set TenantId if user has one
            var user = await _userRepository.GetQueryable().FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                userPermission.TenantId = user.TenantId;
            }

            await _userPermissionRepository.InsertAsync(userPermission);
        }
        else
        {
            userPermission.IsGranted = isGranted;
            await _userPermissionRepository.UpdateAsync(userPermission);
        }
    }

    public async Task RemoveUserPermissionAsync(Guid userId, string permissionName)
    {
        var userPermission = await _userPermissionRepository
            .GetQueryable()
            .FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionName == permissionName);

        if (userPermission != null)
        {
            await _userPermissionRepository.DeleteAsync(userPermission);
        }
    }
}
