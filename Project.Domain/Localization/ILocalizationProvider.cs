namespace Project.Domain.Localization;

/// <summary>
/// Interface for loading localization dictionaries from JSON files.
/// </summary>
public interface ILocalizationProvider
{
    /// <summary>
    /// Loads a localization dictionary from JSON files.
    /// </summary>
    /// <param name="resourceName">Name of the localization resource</param>
    /// <param name="cultureName">Culture name (e.g., "en", "fr")</param>
    /// <returns>Dictionary of localized strings or null if not found</returns>
    Task<Dictionary<string, string>?> LoadDictionaryAsync(string resourceName, string cultureName);

    /// <summary>
    /// Gets all supported cultures for the specified resource.
    /// </summary>
    /// <param name="resourceName">Name of the localization resource</param>
    /// <returns>List of supported culture names</returns>
    Task<List<string>> GetSupportedCulturesAsync(string resourceName);

    /// <summary>
    /// Checks if a culture is supported for the specified resource.
    /// </summary>
    /// <param name="resourceName">Name of the localization resource</param>
    /// <param name="cultureName">Culture name to check</param>
    /// <returns>True if culture is supported, false otherwise</returns>
    Task<bool> IsCultureSupportedAsync(string resourceName, string cultureName);
}
