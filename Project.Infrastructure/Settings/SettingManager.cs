using Project.Core.Interfaces;

namespace Project.Infrastructure.Settings;

/// <summary>
/// Main implementation of ISettingManager with hierarchical resolution.
/// Resolution order: User → Tenant → Global → Default
/// </summary>
public class SettingManager : ISettingManager
{
    private readonly ISettingDefinitionManager _definitionManager;
    private readonly ISettingStore _settingStore;
    private readonly ICurrentUser _currentUser;
    private readonly ICurrentTenant _currentTenant;
    private readonly IEnumerable<ISettingProvider> _providers;

    public SettingManager(
        ISettingDefinitionManager definitionManager,
        ISettingStore settingStore,
        ICurrentUser currentUser,
        ICurrentTenant currentTenant,
        IEnumerable<ISettingProvider> providers)
    {
        _definitionManager = definitionManager;
        _settingStore = settingStore;
        _currentUser = currentUser;
        _currentTenant = currentTenant;
        _providers = providers;
    }

    public async Task<string?> GetOrNullAsync(string name)
    {
        // Try User provider
        if (_currentUser.Id.HasValue)
        {
            var userValue = await _settingStore.GetOrNullAsync(name, "U", _currentUser.Id.Value.ToString());
            if (userValue != null)
                return userValue;
        }

        // Try Tenant provider
        if (_currentTenant.Id.HasValue)
        {
            var tenantValue = await _settingStore.GetOrNullAsync(name, "T", _currentTenant.Id.Value.ToString());
            if (tenantValue != null)
                return tenantValue;
        }

        // Try Global provider
        var globalValue = await _settingStore.GetOrNullAsync(name, "G", null);
        if (globalValue != null)
            return globalValue;

        // Return default value from definition
        var definition = _definitionManager.GetOrNull(name);
        return definition?.DefaultValue;
    }

    public async Task<string> GetAsync(string name)
    {
        var value = await GetOrNullAsync(name);
        if (value == null)
        {
            throw new InvalidOperationException($"Setting '{name}' not found and has no default value.");
        }
        return value;
    }

    public Task<string?> GetForUserAsync(string name, Guid userId)
    {
        return _settingStore.GetOrNullAsync(name, "U", userId.ToString());
    }

    public Task<string?> GetForTenantAsync(string name, Guid tenantId)
    {
        return _settingStore.GetOrNullAsync(name, "T", tenantId.ToString());
    }

    public Task SetGlobalAsync(string name, string value)
    {
        return _settingStore.SetAsync(name, value, "G", null);
    }

    public Task SetForTenantAsync(string name, string value, Guid tenantId)
    {
        return _settingStore.SetAsync(name, value, "T", tenantId.ToString());
    }

    public Task SetForUserAsync(string name, string value, Guid userId)
    {
        return _settingStore.SetAsync(name, value, "U", userId.ToString());
    }

    public async Task DeleteAsync(string name)
    {
        // Delete in order of priority
        if (_currentUser.Id.HasValue)
        {
            await _settingStore.DeleteAsync(name, "U", _currentUser.Id.Value.ToString());
        }
        else if (_currentTenant.Id.HasValue)
        {
            await _settingStore.DeleteAsync(name, "T", _currentTenant.Id.Value.ToString());
        }
        else
        {
            await _settingStore.DeleteAsync(name, "G", null);
        }
    }

    public Task DeleteForUserAsync(string name, Guid userId)
    {
        return _settingStore.DeleteAsync(name, "U", userId.ToString());
    }

    public Task DeleteForTenantAsync(string name, Guid tenantId)
    {
        return _settingStore.DeleteAsync(name, "T", tenantId.ToString());
    }

    public async Task<Dictionary<string, string>> GetAllAsync()
    {
        var result = new Dictionary<string, string>();

        // Start with defaults
        foreach (var definition in _definitionManager.GetAll())
        {
            if (definition.DefaultValue != null)
            {
                result[definition.Name] = definition.DefaultValue;
            }
        }

        // Override with global settings
        var globalSettings = await _settingStore.GetAllAsync("G", null);
        foreach (var kvp in globalSettings)
        {
            result[kvp.Key] = kvp.Value;
        }

        // Override with tenant settings
        if (_currentTenant.Id.HasValue)
        {
            var tenantSettings = await _settingStore.GetAllAsync("T", _currentTenant.Id.Value.ToString());
            foreach (var kvp in tenantSettings)
            {
                result[kvp.Key] = kvp.Value;
            }
        }

        // Override with user settings
        if (_currentUser.Id.HasValue)
        {
            var userSettings = await _settingStore.GetAllAsync("U", _currentUser.Id.Value.ToString());
            foreach (var kvp in userSettings)
            {
                result[kvp.Key] = kvp.Value;
            }
        }

        return result;
    }
}
