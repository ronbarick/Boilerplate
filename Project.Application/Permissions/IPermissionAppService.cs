using System;
using System.Threading.Tasks;
using Project.Application.Common.Dtos;
using Project.Application.Permissions.Dtos;
using Project.Application.Services;
using Project.Core.Interfaces;

namespace Project.Application.Permissions;

public interface IPermissionAppService : IApplicationService
{
    Task<GetPermissionListResultDto> GetAllPermissionsAsync();
    Task<GetPermissionListResultDto> GetRolePermissionsAsync(GetRolePermissionsInput input);
    Task SetRolePermissionsAsync(UpdateRolePermissionsInput input);
    Task<GetPermissionListResultDto> GetUserPermissionsAsync(Guid userId);
    Task SetUserPermissionsAsync(Guid userId, UpdatePermissionsDto input);
}
