using System.Collections.Generic;

namespace Project.Application.AppConfiguration.Dtos;

public class PermissionTreeDto
{
    public List<PermissionGroupDto> Groups { get; set; } = new();
}

public class PermissionGroupDto
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public List<PermissionDto> Permissions { get; set; } = new();
}

public class PermissionDto
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsGranted { get; set; }
}
