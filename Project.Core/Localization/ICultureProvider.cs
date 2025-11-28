namespace Project.Core.Localization;

/// <summary>
/// Interface for resolving the current culture from multiple sources.
/// Resolution order: Query string → Cookie → User Setting → Tenant Setting → Global Setting → Default
/// </summary>
public interface ICultureProvider
{
    /// <summary>
    /// Gets the current culture name for the current request/user.
    /// </summary>
    /// <returns>Culture name (e.g., "en", "fr", "fr-CA")</returns>
    Task<string> GetCurrentCultureAsync();

    /// <summary>
    /// Gets the culture name from the specified source priority.
    /// </summary>
    /// <param name="checkQueryString">Check query string (?culture=en)</param>
    /// <param name="checkCookie">Check culture cookie</param>
    /// <param name="checkUserSetting">Check user-level setting</param>
    /// <param name="checkTenantSetting">Check tenant-level setting</param>
    /// <param name="checkGlobalSetting">Check global-level setting</param>
    /// <returns>Culture name or null if not found in specified sources</returns>
    Task<string?> GetCultureAsync(
        bool checkQueryString = true,
        bool checkCookie = true,
        bool checkUserSetting = true,
        bool checkTenantSetting = true,
        bool checkGlobalSetting = true);

    /// <summary>
    /// Gets the default culture name.
    /// </summary>
    /// <returns>Default culture name (e.g., "en")</returns>
    string GetDefaultCulture();

    /// <summary>
    /// Gets the culture fallback chain for the specified culture.
    /// Example: "fr-CA" → ["fr-CA", "fr", "en"]
    /// </summary>
    /// <param name="cultureName">Culture name</param>
    /// <returns>List of culture names in fallback order</returns>
    List<string> GetCultureFallbackChain(string cultureName);
}
