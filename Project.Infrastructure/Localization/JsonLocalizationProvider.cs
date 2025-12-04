using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Project.Domain.Localization;
using System.Text.Json;

namespace Project.Infrastructure.Localization;

/// <summary>
/// JSON-based localization provider that loads dictionaries from Virtual File System.
/// </summary>
public class JsonLocalizationProvider : ILocalizationProvider
{
    private readonly ILogger<JsonLocalizationProvider> _logger;
    private readonly Dictionary<string, LocalizationResource> _resources;
    private readonly string _basePath;

    public JsonLocalizationProvider(
        ILogger<JsonLocalizationProvider> logger,
        string basePath = "Localization")
    {
        _logger = logger;
        _resources = new Dictionary<string, LocalizationResource>();
        _basePath = basePath;
    }

    /// <summary>
    /// Registers a localization resource.
    /// </summary>
    public void AddResource(LocalizationResource resource)
    {
        _resources[resource.ResourceName] = resource;
    }

    public async Task<Dictionary<string, string>?> LoadDictionaryAsync(string resourceName, string cultureName)
    {
        try
        {
            if (!_resources.TryGetValue(resourceName, out var resource))
            {
                _logger.LogWarning("Localization resource '{ResourceName}' not found", resourceName);
                return null;
            }

            var dictionary = new Dictionary<string, string>();

            // Load base resources first (for inheritance)
            foreach (var baseResource in resource.BaseResources)
            {
                var baseDict = await LoadDictionaryAsync(baseResource.ResourceName, cultureName);
                if (baseDict != null)
                {
                    foreach (var kvp in baseDict)
                    {
                        dictionary[kvp.Key] = kvp.Value;
                    }
                }
            }

            // Load current resource (overwrites base resource keys)
            var currentDict = await LoadJsonFileAsync(resource.VirtualPath, cultureName);
            if (currentDict != null)
            {
                foreach (var kvp in currentDict)
                {
                    dictionary[kvp.Key] = kvp.Value;
                }
            }

            return dictionary.Count > 0 ? dictionary : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading localization dictionary for resource '{ResourceName}' and culture '{CultureName}'",
                resourceName, cultureName);
            return null;
        }
    }

    public async Task<List<string>> GetSupportedCulturesAsync(string resourceName)
    {
        try
        {
            if (!_resources.TryGetValue(resourceName, out var resource))
            {
                return new List<string>();
            }

            var cultures = new List<string>();
            var virtualPath = resource.VirtualPath.TrimStart('/');
            var fullPath = Path.Combine(AppContext.BaseDirectory, virtualPath);

            if (Directory.Exists(fullPath))
            {
                var jsonFiles = Directory.GetFiles(fullPath, "*.json");
                foreach (var file in jsonFiles)
                {
                    var cultureName = Path.GetFileNameWithoutExtension(file);
                    cultures.Add(cultureName);
                }
            }

            return await Task.FromResult(cultures);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting supported cultures for resource '{ResourceName}'", resourceName);
            return new List<string>();
        }
    }

    public async Task<bool> IsCultureSupportedAsync(string resourceName, string cultureName)
    {
        var supportedCultures = await GetSupportedCulturesAsync(resourceName);
        return supportedCultures.Contains(cultureName);
    }

    private async Task<Dictionary<string, string>?> LoadJsonFileAsync(string virtualPath, string cultureName)
    {
        try
        {
            // Remove leading slash and construct full path
            var relativePath = virtualPath.TrimStart('/');
            var fileName = $"{cultureName}.json";
            var fullPath = Path.Combine(AppContext.BaseDirectory, relativePath, fileName);

            if (!File.Exists(fullPath))
            {
                _logger.LogDebug("Localization file not found: {FilePath}", fullPath);
                return null;
            }

            var json = await File.ReadAllTextAsync(fullPath);
            var dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

            _logger.LogInformation("Loaded {Count} localization strings from {FilePath}",
                dictionary?.Count ?? 0, fullPath);

            return dictionary;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading JSON file from path '{VirtualPath}' for culture '{CultureName}'",
                virtualPath, cultureName);
            return null;
        }
    }
}
