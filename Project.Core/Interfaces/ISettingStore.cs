namespace Project.Core.Interfaces;

/// <summary>
/// Stores and retrieves setting values from the database.
/// </summary>
public interface ISettingStore
{
    /// <summary>
    /// Gets a setting value or null if not found.
    /// </summary>
    Task<string?> GetOrNullAsync(string name, string providerName, string? providerKey);

    /// <summary>
    /// Sets a setting value.
    /// </summary>
    Task SetAsync(string name, string value, string providerName, string? providerKey);

    /// <summary>
    /// Deletes a setting.
    /// </summary>
    Task DeleteAsync(string name, string providerName, string? providerKey);

    /// <summary>
    /// Gets all settings for a provider.
    /// </summary>
    Task<Dictionary<string, string>> GetAllAsync(string providerName, string? providerKey);
}
