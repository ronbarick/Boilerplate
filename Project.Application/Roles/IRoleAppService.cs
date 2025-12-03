using System;
using System.Threading.Tasks;
using Project.Application.Common.Dtos;
using Project.Application.Roles.Dtos;
using Project.Application.Services;
using Project.Domain.Interfaces;

namespace Project.Application.Roles;

public interface IRoleAppService : IApplicationService
{
    Task<PagedResultDto<RoleDto>> GetListAsync(GetRolesInput input);
    Task<RoleDto> GetAsync(Guid id);
    Task<RoleDto> CreateAsync(CreateRoleDto input);
    Task<RoleDto> UpdateAsync(Guid id, UpdateRoleDto input);
    Task DeleteAsync(Guid id);
}
