namespace Project.Core.Interfaces;

/// <summary>
/// Provides setting values for a specific provider (User, Tenant, Global).
/// </summary>
public interface ISettingProvider
{
    /// <summary>
    /// Provider name: "U", "T", or "G"
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets a setting value or null if not found.
    /// </summary>
    Task<string?> GetOrNullAsync(string settingName);
}
