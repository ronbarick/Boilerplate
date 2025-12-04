namespace Project.Domain.Localization;

/// <summary>
/// Main localization resource for the Project application.
/// </summary>
public static class ProjectLocalizationResource
{
    /// <summary>
    /// Resource name constant
    /// </summary>
    public const string ResourceName = "Project";

    /// <summary>
    /// Virtual path to localization JSON files
    /// </summary>
    public const string VirtualPath = "/Localization/Project";

    /// <summary>
    /// Gets the localization resource definition
    /// </summary>
    public static LocalizationResource Resource { get; } = new LocalizationResource(
        ResourceName,
        VirtualPath
    );
}
