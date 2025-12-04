using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Project.Domain.Localization;
using System.Text.Json;

namespace Project.Infrastructure.Localization;

/// <summary>
/// Main localization manager with caching and fallback support.
/// </summary>
public class LocalizationManager : ILocalizationManager
{
    private readonly ILocalizationProvider _provider;
    private readonly ICultureProvider _cultureProvider;
    private readonly IDistributedCache _cache;
    private readonly ILogger<LocalizationManager> _logger;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromHours(1);

    public LocalizationManager(
        ILocalizationProvider provider,
        ICultureProvider cultureProvider,
        IDistributedCache cache,
        ILogger<LocalizationManager> logger)
    {
        _provider = provider;
        _cultureProvider = cultureProvider;
        _cache = cache;
        _logger = logger;
    }

    public async Task<string> GetStringAsync(string resourceName, string key, params object[] args)
    {
        var cultureName = await _cultureProvider.GetCurrentCultureAsync();
        return await GetStringAsync(resourceName, key, cultureName, args);
    }

    public async Task<string> GetStringAsync(string resourceName, string key, string cultureName, params object[] args)
    {
        var value = await GetStringOrNullAsync(resourceName, key, cultureName);

        if (value == null)
        {
            _logger.LogWarning("Localization key '{Key}' not found for resource '{ResourceName}' and culture '{CultureName}'",
                key, resourceName, cultureName);
            value = $"[{key}]";
        }

        // Apply formatting if args are provided
        if (args != null && args.Length > 0)
        {
            try
            {
                value = string.Format(value, args);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error formatting localized string for key '{Key}'", key);
            }
        }

        return value;
    }

    public async Task<string?> GetStringOrNullAsync(string resourceName, string key, string? cultureName = null)
    {
        cultureName ??= await _cultureProvider.GetCurrentCultureAsync();

        // Try culture fallback chain
        var fallbackChain = _cultureProvider.GetCultureFallbackChain(cultureName);

        foreach (var culture in fallbackChain)
        {
            var dictionary = await GetOrLoadDictionaryAsync(resourceName, culture);
            if (dictionary != null && dictionary.TryGetValue(key, out var value))
            {
                return value;
            }
        }

        return null;
    }

    public async Task<Dictionary<string, string>> GetAllStringsAsync(string resourceName, string? cultureName = null)
    {
        cultureName ??= await _cultureProvider.GetCurrentCultureAsync();

        var result = new Dictionary<string, string>();

        // Load all dictionaries in fallback chain and merge (later cultures override earlier ones)
        var fallbackChain = _cultureProvider.GetCultureFallbackChain(cultureName);
        fallbackChain.Reverse(); // Start from default culture

        foreach (var culture in fallbackChain)
        {
            var dictionary = await GetOrLoadDictionaryAsync(resourceName, culture);
            if (dictionary != null)
            {
                foreach (var kvp in dictionary)
                {
                    result[kvp.Key] = kvp.Value;
                }
            }
        }

        return result;
    }

    public async Task<List<string>> GetSupportedCulturesAsync(string resourceName)
    {
        return await _provider.GetSupportedCulturesAsync(resourceName);
    }

    private async Task<Dictionary<string, string>?> GetOrLoadDictionaryAsync(string resourceName, string cultureName)
    {
        var cacheKey = GetCacheKey(resourceName, cultureName);

        // Try to get from cache
        var cachedJson = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedJson))
        {
            try
            {
                return JsonSerializer.Deserialize<Dictionary<string, string>>(cachedJson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deserializing cached localization dictionary for key '{CacheKey}'", cacheKey);
            }
        }

        // Load from provider
        var dictionary = await _provider.LoadDictionaryAsync(resourceName, cultureName);

        // Cache if found
        if (dictionary != null)
        {
            try
            {
                var json = JsonSerializer.Serialize(dictionary);
                var options = new DistributedCacheEntryOptions
                {
                    SlidingExpiration = _cacheExpiration
                };
                await _cache.SetStringAsync(cacheKey, json, options);

                _logger.LogDebug("Cached localization dictionary for resource '{ResourceName}' and culture '{CultureName}'",
                    resourceName, cultureName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error caching localization dictionary for key '{CacheKey}'", cacheKey);
            }
        }

        return dictionary;
    }

    private static string GetCacheKey(string resourceName, string cultureName)
    {
        return $"L:{resourceName}:{cultureName}";
    }
}
