using Microsoft.EntityFrameworkCore;
using Project.Application.AppConfiguration.Dtos;
using Project.Application.Services;
using Project.Domain.Entities;
using Project.Domain.Interfaces;
using Project.Domain.Localization;

namespace Project.Application.AppConfiguration;

public class ProfileAppService : AppServiceBase, IProfileAppService
{
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Role> _roleRepository;
    private readonly IRepository<UserRole> _userRoleRepository;
    private readonly IRepository<Tenant> _tenantRepository;
    private readonly IRepository<RolePermission> _rolePermissionRepository;
    private readonly IRepository<UserPermission> _userPermissionRepository;
    private readonly IRepository<PermissionDefinition> _permissionRepository;

    public ProfileAppService(
        ICurrentUser currentUser,
        ICurrentTenant currentTenant,
        IPermissionChecker permissionChecker,
        ILocalizationManager localizationManager,
        IRepository<User> userRepository,
        IRepository<Role> roleRepository,
        IRepository<UserRole> userRoleRepository,
        IRepository<Tenant> tenantRepository,
        IRepository<RolePermission> rolePermissionRepository,
        IRepository<UserPermission> userPermissionRepository,
        IRepository<PermissionDefinition> permissionRepository)
        : base(currentUser, currentTenant, permissionChecker, localizationManager)
    {
        _currentUser = currentUser;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _tenantRepository = tenantRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _userPermissionRepository = userPermissionRepository;
        _permissionRepository = permissionRepository;
    }

    public async Task<MyProfileDto> GetMyProfileAsync()
    {
        if (!_currentUser.IsAuthenticated || !_currentUser.Id.HasValue)
            throw new UnauthorizedAccessException("User is not authenticated");

        var user = await _userRepository.GetAsync(_currentUser.Id.Value);
        if (user == null)
            throw new InvalidOperationException("User not found");

        // Get user roles
        var userRoles = await _userRoleRepository.GetQueryable()
            .Where(ur => ur.UserId == user.Id)
            .Join(_roleRepository.GetQueryable(),
                ur => ur.RoleId,
                r => r.Id,
                (ur, r) => r.Name)
            .ToListAsync();

        // Get granted permissions
        var permissions = await GetGrantedPermissionsAsync(user.Id, userRoles);

        // Get tenant name if applicable
        string? tenantName = null;
        if (user.TenantId.HasValue)
        {
            var tenant = await _tenantRepository.GetAsync(user.TenantId.Value);
            tenantName = tenant?.Name;
        }

        return new MyProfileDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Name = user.Name,
            Surname = user.Surname,
            Email = user.EmailAddress,
            PhoneNumber = user.PhoneNumber,
            IsEmailConfirmed = user.IsEmailConfirmed,
            Roles = userRoles,
            Permissions = permissions,
            TenantId = user.TenantId,
            TenantName = tenantName
        };
    }

    private async Task<List<string>> GetGrantedPermissionsAsync(Guid userId, List<string> roleNames)
    {
        var permissions = new HashSet<string>();

        // Get role IDs
        var roleIds = await _roleRepository.GetQueryable()
            .Where(r => roleNames.Contains(r.Name))
            .Select(r => r.Id)
            .ToListAsync();

        // Get role permissions
        var rolePermissions = await _rolePermissionRepository.GetQueryable()
            .Where(rp => roleIds.Contains(rp.RoleId) && rp.IsGranted)
            .Select(rp => rp.PermissionName)
            .ToListAsync();

        foreach (var perm in rolePermissions)
        {
            permissions.Add(perm);
        }

        // Get user-specific permissions
        var userPermissions = await _userPermissionRepository.GetQueryable()
            .Where(up => up.UserId == userId && up.IsGranted)
            .Select(up => up.PermissionName)
            .ToListAsync();

        foreach (var perm in userPermissions)
        {
            permissions.Add(perm);
        }

        return permissions.ToList();
    }
}
