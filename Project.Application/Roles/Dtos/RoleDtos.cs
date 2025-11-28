using System;
using Project.Application.Common.Dtos;

namespace Project.Application.Roles.Dtos;

public class RoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public bool IsStatic { get; set; }
    public bool IsDefault { get; set; }
    public DateTime CreatedOn { get; set; }
}

public class CreateRoleDto
{
    public string Name { get; set; } = null!;
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public bool IsDefault { get; set; } = false;
}

public class UpdateRoleDto
{
    public string Name { get; set; } = null!;
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public bool IsDefault { get; set; }
}


