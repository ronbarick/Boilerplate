namespace Project.Core.Localization;

/// <summary>
/// Constants for localization setting names.
/// </summary>
public static class LocalizationSettingNames
{
    /// <summary>
    /// Current culture for the user/tenant.
    /// Example: "en", "fr", "fr-CA"
    /// </summary>
    public const string CurrentCulture = "App.Localization.CurrentCulture";

    /// <summary>
    /// Comma-separated list of supported cultures.
    /// Example: "en,fr,es"
    /// </summary>
    public const string SupportedCultures = "App.Localization.SupportedCultures";

    /// <summary>
    /// Default culture when no other culture is specified.
    /// Example: "en"
    /// </summary>
    public const string DefaultCulture = "App.Localization.DefaultCulture";
}
