using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.Core.Interfaces;

/// <summary>
/// Permission manager for managing role and user permissions
/// </summary>
public interface IPermissionManager
{
    // Role Permission Methods
    
    /// <summary>
    /// Gets all permissions for a role by role ID
    /// </summary>
    Task<Dictionary<string, bool>> GetForRoleAsync(Guid roleId);
    
    /// <summary>
    /// Gets all permissions for a role by role name
    /// </summary>
    Task<Dictionary<string, bool>> GetForRoleAsync(string roleName);
    
    /// <summary>
    /// Sets a permission for a role by role ID
    /// </summary>
    Task SetForRoleAsync(Guid roleId, string permissionName, bool isGranted);
    
    /// <summary>
    /// Sets a permission for a role by role name
    /// </summary>
    Task SetForRoleAsync(string roleName, string permissionName, bool isGranted);
    
    /// <summary>
    /// Grants all specified permissions to a role by role ID
    /// </summary>
    Task GrantAllAsync(Guid roleId, string[] permissionNames);
    
    /// <summary>
    /// Grants all specified permissions to a role by role name
    /// </summary>
    Task GrantAllAsync(string roleName, string[] permissionNames);
    
    // User Permission Methods
    
    /// <summary>
    /// Gets all permissions for a user (including inherited from roles)
    /// </summary>
    Task<Dictionary<string, bool>> GetForUserAsync(Guid userId);
    
    /// <summary>
    /// Sets a permission for a user (overrides role permissions)
    /// </summary>
    Task SetForUserAsync(Guid userId, string permissionName, bool isGranted);
    
    /// <summary>
    /// Removes a user-level permission override
    /// </summary>
    Task RemoveUserPermissionAsync(Guid userId, string permissionName);
}
