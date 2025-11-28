using System;
using System.Collections.Generic;

namespace Project.Application.Permissions.Dtos;

/// <summary>
/// Represents a permission with its grant state and hierarchy information
/// </summary>
public class PermissionDto
{
    public string Name { get; set; } = null!;
    public string? DisplayName { get; set; }
    public string? ParentName { get; set; }
    public bool IsGranted { get; set; }
    public PermissionGrantState GrantState { get; set; }
    public List<PermissionDto> Children { get; set; } = new();
}

/// <summary>
/// Permission grant state
/// </summary>
public enum PermissionGrantState
{
    /// <summary>
    /// Permission is not granted
    /// </summary>
    NotGranted = 0,
    
    /// <summary>
    /// Permission is explicitly granted
    /// </summary>
    Granted = 1,
    
    /// <summary>
    /// Permission is explicitly revoked (denied)
    /// </summary>
    Revoked = 2,
    
    /// <summary>
    /// Permission is inherited from a role
    /// </summary>
    Inherited = 3
}

/// <summary>
/// Result DTO for getting permission list with hierarchy
/// </summary>
public class GetPermissionListResultDto
{
    public string EntityDisplayName { get; set; } = null!;
    public List<PermissionGroupDto> Groups { get; set; } = new();
}

/// <summary>
/// Permission group for organizing permissions
/// </summary>
public class PermissionGroupDto
{
    public string Name { get; set; } = null!;
    public string? DisplayName { get; set; }
    public List<PermissionDto> Permissions { get; set; } = new();
}

/// <summary>
/// Input DTO for updating permissions
/// </summary>
public class UpdatePermissionsDto
{
    public List<UpdatePermissionDto> Permissions { get; set; } = new();
}

/// <summary>
/// Single permission update
/// </summary>
public class UpdatePermissionDto
{
    public string Name { get; set; } = null!;
    public bool IsGranted { get; set; }
}

/// <summary>
/// Input for getting role permissions
/// </summary>
public class GetRolePermissionsInput
{
    public Guid? RoleId { get; set; }
    public string? RoleName { get; set; }
}

/// <summary>
/// Input for updating role permissions
/// </summary>
public class UpdateRolePermissionsInput
{
    public Guid? RoleId { get; set; }
    public string? RoleName { get; set; }
    public List<UpdatePermissionDto> Permissions { get; set; } = new();
}
