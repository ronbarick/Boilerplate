using Project.Domain.Localization;

namespace Project.Domain.Extensions;

/// <summary>
/// Extension methods for convenient localization access.
/// </summary>
public static class LocalizationExtensions
{
    /// <summary>
    /// Gets a localized string for the Project resource.
    /// </summary>
    /// <param name="manager">Localization manager</param>
    /// <param name="key">Localization key</param>
    /// <param name="args">Optional format arguments</param>
    /// <returns>Localized string</returns>
    public static Task<string> L(this ILocalizationManager manager, string key, params object[] args)
    {
        return manager.GetStringAsync(ProjectLocalizationResource.ResourceName, key, args);
    }

    /// <summary>
    /// Gets a localized string for a specific resource.
    /// </summary>
    /// <param name="manager">Localization manager</param>
    /// <param name="resourceName">Resource name</param>
    /// <param name="key">Localization key</param>
    /// <param name="args">Optional format arguments</param>
    /// <returns>Localized string</returns>
    public static Task<string> L(this ILocalizationManager manager, string resourceName, string key, params object[] args)
    {
        return manager.GetStringAsync(resourceName, key, args);
    }

    /// <summary>
    /// Gets a localized string for a specific culture.
    /// </summary>
    /// <param name="manager">Localization manager</param>
    /// <param name="key">Localization key</param>
    /// <param name="cultureName">Culture name</param>
    /// <param name="args">Optional format arguments</param>
    /// <returns>Localized string</returns>
    public static Task<string> LWithCulture(this ILocalizationManager manager, string key, string cultureName, params object[] args)
    {
        return manager.GetStringAsync(ProjectLocalizationResource.ResourceName, key, cultureName, args);
    }
}
