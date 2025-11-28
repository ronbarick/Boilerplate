namespace Project.Core.Interfaces;

/// <summary>
/// Main API for managing settings with hierarchical resolution.
/// Resolution order: User → Tenant → Global → Default
/// </summary>
public interface ISettingManager
{
    /// <summary>
    /// Gets a setting value using the fallback chain, or null if not found.
    /// </summary>
    Task<string?> GetOrNullAsync(string name);

    /// <summary>
    /// Gets a setting value using the fallback chain.
    /// Throws if not found and no default value.
    /// </summary>
    Task<string> GetAsync(string name);

    /// <summary>
    /// Gets a setting value for a specific user.
    /// </summary>
    Task<string?> GetForUserAsync(string name, Guid userId);

    /// <summary>
    /// Gets a setting value for a specific tenant.
    /// </summary>
    Task<string?> GetForTenantAsync(string name, Guid tenantId);

    /// <summary>
    /// Sets a global setting.
    /// </summary>
    Task SetGlobalAsync(string name, string value);

    /// <summary>
    /// Sets a tenant-specific setting.
    /// </summary>
    Task SetForTenantAsync(string name, string value, Guid tenantId);

    /// <summary>
    /// Sets a user-specific setting.
    /// </summary>
    Task SetForUserAsync(string name, string value, Guid userId);

    /// <summary>
    /// Deletes a setting for the current context (user/tenant/global).
    /// </summary>
    Task DeleteAsync(string name);

    /// <summary>
    /// Deletes a user-specific setting.
    /// </summary>
    Task DeleteForUserAsync(string name, Guid userId);

    /// <summary>
    /// Deletes a tenant-specific setting.
    /// </summary>
    Task DeleteForTenantAsync(string name, Guid tenantId);

    /// <summary>
    /// Gets all settings as a dictionary.
    /// </summary>
    Task<Dictionary<string, string>> GetAllAsync();
}
