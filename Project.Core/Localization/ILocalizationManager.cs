namespace Project.Core.Localization;

/// <summary>
/// Main API for retrieving localized strings with fallback support.
/// </summary>
public interface ILocalizationManager
{
    /// <summary>
    /// Gets a localized string for the specified resource and key.
    /// Uses current culture from ICultureProvider.
    /// </summary>
    /// <param name="resourceName">Name of the localization resource</param>
    /// <param name="key">Localization key</param>
    /// <param name="args">Optional format arguments</param>
    /// <returns>Localized string or key in brackets if not found</returns>
    Task<string> GetStringAsync(string resourceName, string key, params object[] args);

    /// <summary>
    /// Gets a localized string for the specified resource, key, and culture.
    /// </summary>
    /// <param name="resourceName">Name of the localization resource</param>
    /// <param name="key">Localization key</param>
    /// <param name="cultureName">Culture name (e.g., "en", "fr")</param>
    /// <param name="args">Optional format arguments</param>
    /// <returns>Localized string or key in brackets if not found</returns>
    Task<string> GetStringAsync(string resourceName, string key, string cultureName, params object[] args);

    /// <summary>
    /// Gets a localized string or null if not found.
    /// </summary>
    /// <param name="resourceName">Name of the localization resource</param>
    /// <param name="key">Localization key</param>
    /// <param name="cultureName">Culture name (optional, uses current culture if null)</param>
    /// <returns>Localized string or null</returns>
    Task<string?> GetStringOrNullAsync(string resourceName, string key, string? cultureName = null);

    /// <summary>
    /// Gets all localized strings for the specified resource and culture.
    /// </summary>
    /// <param name="resourceName">Name of the localization resource</param>
    /// <param name="cultureName">Culture name (optional, uses current culture if null)</param>
    /// <returns>Dictionary of all localized strings</returns>
    Task<Dictionary<string, string>> GetAllStringsAsync(string resourceName, string? cultureName = null);

    /// <summary>
    /// Gets all supported cultures for the specified resource.
    /// </summary>
    /// <param name="resourceName">Name of the localization resource</param>
    /// <returns>List of supported culture names</returns>
    Task<List<string>> GetSupportedCulturesAsync(string resourceName);
}
