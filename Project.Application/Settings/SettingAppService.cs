using Project.Application.Services;
using Project.Application.Settings.Dtos;
using Project.Domain.Interfaces;
using Project.Domain.Localization;

namespace Project.Application.Settings;

/// <summary>
/// Application service for managing settings.
/// </summary>
public class SettingAppService : AppServiceBase, IApplicationService
{
    private readonly ISettingManager _settingManager;
    private readonly ISettingDefinitionManager _definitionManager;

    public SettingAppService(
        ICurrentUser currentUser,
        ICurrentTenant currentTenant,
        IPermissionChecker permissionChecker,
        ISettingManager settingManager,
        ISettingDefinitionManager definitionManager,
        ILocalizationManager localizationManager)
        : base(currentUser, currentTenant, permissionChecker, localizationManager)
    {
        _settingManager = settingManager;
        _definitionManager = definitionManager;
    }

    /// <summary>
    /// Gets all settings for the current context.
    /// </summary>
    public async Task<List<SettingDto>> GetAllAsync()
    {
        var settings = await _settingManager.GetAllAsync();
        return settings.Select(kvp => new SettingDto
        {
            Name = kvp.Key,
            Value = kvp.Value
        }).ToList();
    }

    /// <summary>
    /// Gets a specific setting value.
    /// </summary>
    public async Task<SettingValueDto> GetAsync(string name)
    {
        var value = await _settingManager.GetOrNullAsync(name);
        return new SettingValueDto { Value = value ?? string.Empty };
    }

    /// <summary>
    /// Sets a setting value for the current user.
    /// </summary>
    public async Task SetForCurrentUserAsync(SetSettingDto input)
    {
        if (!CurrentUser.Id.HasValue)
            throw new UnauthorizedAccessException("User must be authenticated.");

        await _settingManager.SetForUserAsync(input.Name, input.Value, CurrentUser.Id.Value);
    }

    /// <summary>
    /// Sets a setting value for the current tenant.
    /// </summary>
    public async Task SetForCurrentTenantAsync(SetSettingDto input)
    {
        if (!CurrentTenant.Id.HasValue)
            throw new UnauthorizedAccessException("Tenant context required.");

        await _settingManager.SetForTenantAsync(input.Name, input.Value, CurrentTenant.Id.Value);
    }

    /// <summary>
    /// Sets a global setting.
    /// </summary>
    public async Task SetGlobalAsync(SetSettingDto input)
    {
        // TODO: Add permission check for global settings
        await _settingManager.SetGlobalAsync(input.Name, input.Value);
    }

    /// <summary>
    /// Deletes a setting for the current context.
    /// </summary>
    public async Task DeleteAsync(string name)
    {
        await _settingManager.DeleteAsync(name);
    }
}
