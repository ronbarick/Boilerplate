namespace Project.Domain.Localization;

/// <summary>
/// Represents a localization resource definition.
/// </summary>
public class LocalizationResource
{
    /// <summary>
    /// Unique name of the resource (e.g., "Project", "Common")
    /// </summary>
    public string ResourceName { get; }

    /// <summary>
    /// Virtual path to JSON files (e.g., "/Localization/Project")
    /// </summary>
    public string VirtualPath { get; }

    /// <summary>
    /// Base resources for inheritance (optional)
    /// </summary>
    public List<LocalizationResource> BaseResources { get; }

    public LocalizationResource(
        string resourceName,
        string virtualPath,
        List<LocalizationResource>? baseResources = null)
    {
        ResourceName = resourceName ?? throw new ArgumentNullException(nameof(resourceName));
        VirtualPath = virtualPath ?? throw new ArgumentNullException(nameof(virtualPath));
        BaseResources = baseResources ?? new List<LocalizationResource>();
    }

    /// <summary>
    /// Adds a base resource for inheritance.
    /// </summary>
    public LocalizationResource AddBaseResource(LocalizationResource baseResource)
    {
        BaseResources.Add(baseResource);
        return this;
    }
}
