using System;
using System.Threading.Tasks;
using Project.Application.Common.Dtos;
using Project.Application.Users.Dtos;
using Project.Domain.Interfaces;

namespace Project.Application.Users;

public interface IUserAppService : IApplicationService
{
    Task<PagedResultDto<UserDto>> GetListAsync(GetUsersInput input);
    Task<UserDto> GetAsync(Guid id);
    Task<UserDto> CreateAsync(CreateUserDto input);
    Task<UserDto> UpdateAsync(Guid id, UpdateUserDto input);
    Task DeleteAsync(Guid id);
    
    // Password management operations
    Task ForgotPasswordAsync(ForgotPasswordDto input);
    Task ResetPasswordAsync(ResetPasswordDto input);
    Task SetPasswordAsync(SetPasswordDto input);
    
    // Email confirmation operations
    Task ConfirmEmailAsync(ConfirmEmailDto input);
    Task ResendConfirmationEmailAsync(Guid userId);
}
