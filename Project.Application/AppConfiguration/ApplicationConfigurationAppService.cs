using System.Linq;
using Microsoft.EntityFrameworkCore;
using Project.Application.AppConfiguration.Dtos;
using Project.Application.Services;
using Project.Domain.Entities;
using Project.Domain.Interfaces;
using Project.Domain.Localization;

namespace Project.Application.AppConfiguration;

public class ApplicationConfigurationAppService : AppServiceBase, IApplicationConfigurationAppService
{
    private readonly ICurrentUser _currentUser;
    private readonly ICurrentTenant _currentTenant;
    private readonly IPermissionChecker _permissionChecker;
    private readonly IFeatureChecker _featureChecker;
    private readonly ISettingManager _settingManager;
    private readonly ILocalizationProvider _localizationProvider;
    private readonly ICultureProvider _cultureProvider;
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Role> _roleRepository;
    private readonly IRepository<UserRole> _userRoleRepository;
    private readonly IRepository<Tenant> _tenantRepository;
    private readonly IRepository<PermissionDefinition> _permissionRepository;
    private readonly IRepository<RolePermission> _rolePermissionRepository;
    private readonly IRepository<UserPermission> _userPermissionRepository;

    public ApplicationConfigurationAppService(
        ICurrentUser currentUser,
        ICurrentTenant currentTenant,
        IPermissionChecker permissionChecker,
        ILocalizationManager localizationManager,
        IFeatureChecker featureChecker,
        ISettingManager settingManager,
        ILocalizationProvider localizationProvider,
        ICultureProvider cultureProvider,
        IRepository<User> userRepository,
        IRepository<Role> roleRepository,
        IRepository<UserRole> userRoleRepository,
        IRepository<Tenant> tenantRepository,
        IRepository<PermissionDefinition> permissionRepository,
        IRepository<RolePermission> rolePermissionRepository,
        IRepository<UserPermission> userPermissionRepository)
        : base(currentUser, currentTenant, permissionChecker, localizationManager)
    {
        _currentUser = currentUser;
        _currentTenant = currentTenant;
        _permissionChecker = permissionChecker;
        _featureChecker = featureChecker;
        _settingManager = settingManager;
        _localizationProvider = localizationProvider;
        _cultureProvider = cultureProvider;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _tenantRepository = tenantRepository;
        _permissionRepository = permissionRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _userPermissionRepository = userPermissionRepository;
    }

    public async Task<ApplicationConfigurationDto> GetAsync()
    {
        // Execute independent queries in parallel for better performance
        var localizationTask = GetLocalizationConfigurationAsync();
        var authTask = GetAuthConfigurationAsync();
        var multiTenancyConfig = GetMultiTenancyConfiguration();

        await Task.WhenAll(localizationTask, authTask);

        var config = new ApplicationConfigurationDto
        {
            Localization = await localizationTask,
            Auth = await authTask,
            MultiTenancy = multiTenancyConfig
        };

        // Execute user-specific queries in parallel if authenticated
        if (_currentUser.IsAuthenticated)
        {
            var currentUserTask = GetCurrentUserAsync();
            var featuresTask = GetFeaturesConfigurationAsync();
            var settingsTask = GetSettingsAsync();

            await Task.WhenAll(currentUserTask, featuresTask, settingsTask);

            config.CurrentUser = await currentUserTask;
            config.Features = await featuresTask;
            config.Settings = await settingsTask;
        }
        else
        {
            config.Features = new FeaturesConfigurationDto();
            config.Settings = new Dictionary<string, string>();
        }

        return config;
    }

    private async Task<LocalizationConfigurationDto> GetLocalizationConfigurationAsync()
    {
        var currentCulture = await _cultureProvider.GetCurrentCultureAsync();

        return new LocalizationConfigurationDto
        {
            CurrentCulture = new CurrentCultureDto
            {
                Name = currentCulture,
                DisplayName = currentCulture
            },
            Languages = new List<LanguageDto>
            {
                new() { CultureName = "en", DisplayName = "English", IsDefault = true },
                new() { CultureName = "fr", DisplayName = "Français", IsDefault = false }
            },
            Resources = new List<string> { "Project" }
        };
    }

    private async Task<AuthConfigurationDto> GetAuthConfigurationAsync()
    {
        var grantedPolicies = new Dictionary<string, bool>();

        if (_currentUser.IsAuthenticated && _currentUser.Id.HasValue)
        {
            // Optimized: Get all data in batch queries instead of N+1
            // Get user's role IDs
            var roleIds = await _userRoleRepository.GetQueryable()
                .Where(ur => ur.UserId == _currentUser.Id.Value)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            // Get ALL granted permissions from roles in ONE query
            var roleGrantedPermissions = await _rolePermissionRepository.GetQueryable()
                .Where(rp => roleIds.Contains(rp.RoleId) && rp.IsGranted)
                .Select(rp => rp.PermissionName)
                .Distinct()
                .ToListAsync();

            // Get user-specific permission overrides
            var userPermissions = await _userPermissionRepository.GetQueryable()
                .Where(up => up.UserId == _currentUser.Id.Value)
                .Select(up => new { up.PermissionName, up.IsGranted })
                .ToListAsync();

            // Get all permission definitions
            var allPermissions = await _permissionRepository.GetQueryable()
                .Select(p => p.Name)
                .ToListAsync();

            // Build granted policies dictionary
            var rolePermissionSet = new HashSet<string>(roleGrantedPermissions);
            var userPermissionDict = userPermissions.ToDictionary(up => up.PermissionName, up => up.IsGranted);

            foreach (var permission in allPermissions)
            {
                // User-specific permission overrides role permission
                if (userPermissionDict.TryGetValue(permission, out var userGranted))
                {
                    grantedPolicies[permission] = userGranted;
                }
                else
                {
                    grantedPolicies[permission] = rolePermissionSet.Contains(permission);
                }
            }
        }

        return new AuthConfigurationDto
        {
            GrantedPolicies = grantedPolicies,
            TwoFactorEnabled = true
        };
    }

    private MultiTenancyConfigurationDto GetMultiTenancyConfiguration()
    {
        return new MultiTenancyConfigurationDto
        {
            IsEnabled = true
        };
    }

    private async Task<CurrentUserDto?> GetCurrentUserAsync()
    {
        if (!_currentUser.IsAuthenticated || !_currentUser.Id.HasValue)
            return null;

        var user = await _userRepository.GetAsync(_currentUser.Id.Value);
        if (user == null)
            return null;

        // Get user roles
        var userRoles = await _userRoleRepository.GetQueryable()
            .Where(ur => ur.UserId == user.Id)
            .Join(_roleRepository.GetQueryable(),
                ur => ur.RoleId,
                r => r.Id,
                (ur, r) => r.Name)
            .ToListAsync();

        return new CurrentUserDto
        {
            IsAuthenticated = true,
            Id = user.Id,
            TenantId = user.TenantId,
            UserName = user.UserName,
            Name = user.Name,
            Surname = user.Surname,
            Email = user.EmailAddress,
            Roles = userRoles
        };
    }

    private async Task<FeaturesConfigurationDto> GetFeaturesConfigurationAsync()
    {
        var features = new Dictionary<string, string>();

        if (_currentUser.IsAuthenticated)
        {
            // Get all feature values for current tenant/user
            // This is a simplified version - you may want to get all defined features
            var featureNames = new[] { "MaxStudentCount", "EnableReporting" };
            
            foreach (var featureName in featureNames)
            {
                var value = await _featureChecker.GetValueAsync(featureName);
                if (!string.IsNullOrEmpty(value))
                {
                    features[featureName] = value;
                }
            }
        }

        return new FeaturesConfigurationDto
        {
            Values = features
        };
    }

    private async Task<Dictionary<string, string>> GetSettingsAsync()
    {
        if (_currentUser.IsAuthenticated)
        {
            // Get all settings for current user/tenant
            return await _settingManager.GetAllAsync();
        }

        return new Dictionary<string, string>();
    }
}
