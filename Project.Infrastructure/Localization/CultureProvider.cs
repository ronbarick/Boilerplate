using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Project.Core.Interfaces;
using Project.Core.Localization;
using System.Globalization;

namespace Project.Infrastructure.Localization;

/// <summary>
/// Resolves culture from multiple sources with priority:
/// Query string → Cookie → User Setting → Tenant Setting → Global Setting → Default
/// </summary>
public class CultureProvider : ICultureProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ISettingManager _settingManager;
    private readonly ICurrentUser _currentUser;
    private readonly ICurrentTenant _currentTenant;
    private readonly ILogger<CultureProvider> _logger;
    private const string DefaultCultureName = "en";
    private const string CultureQueryStringKey = "culture";
    private const string CultureCookieKey = ".AspNetCore.Culture";

    public CultureProvider(
        IHttpContextAccessor httpContextAccessor,
        ISettingManager settingManager,
        ICurrentUser currentUser,
        ICurrentTenant currentTenant,
        ILogger<CultureProvider> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _settingManager = settingManager;
        _currentUser = currentUser;
        _currentTenant = currentTenant;
        _logger = logger;
    }

    public async Task<string> GetCurrentCultureAsync()
    {
        var culture = await GetCultureAsync();
        return culture ?? GetDefaultCulture();
    }

    public async Task<string?> GetCultureAsync(
        bool checkQueryString = true,
        bool checkCookie = true,
        bool checkUserSetting = true,
        bool checkTenantSetting = true,
        bool checkGlobalSetting = true)
    {
        // 1. Check query string
        if (checkQueryString)
        {
            var cultureFromQuery = GetCultureFromQueryString();
            if (!string.IsNullOrEmpty(cultureFromQuery))
            {
                _logger.LogDebug("Culture resolved from query string: {Culture}", cultureFromQuery);
                return cultureFromQuery;
            }
        }

        // 2. Check cookie
        if (checkCookie)
        {
            var cultureFromCookie = GetCultureFromCookie();
            if (!string.IsNullOrEmpty(cultureFromCookie))
            {
                _logger.LogDebug("Culture resolved from cookie: {Culture}", cultureFromCookie);
                return cultureFromCookie;
            }
        }

        // 3. Check user setting
        if (checkUserSetting && _currentUser.Id.HasValue)
        {
            var cultureFromUserSetting = await _settingManager.GetForUserAsync(
                LocalizationSettingNames.CurrentCulture,
                _currentUser.Id.Value);

            if (!string.IsNullOrEmpty(cultureFromUserSetting))
            {
                _logger.LogDebug("Culture resolved from user setting: {Culture}", cultureFromUserSetting);
                return cultureFromUserSetting;
            }
        }

        // 4. Check tenant setting
        if (checkTenantSetting && _currentTenant.Id.HasValue)
        {
            var cultureFromTenantSetting = await _settingManager.GetForTenantAsync(
                LocalizationSettingNames.CurrentCulture,
                _currentTenant.Id.Value);

            if (!string.IsNullOrEmpty(cultureFromTenantSetting))
            {
                _logger.LogDebug("Culture resolved from tenant setting: {Culture}", cultureFromTenantSetting);
                return cultureFromTenantSetting;
            }
        }

        // 5. Check global setting
        if (checkGlobalSetting)
        {
            var cultureFromGlobalSetting = await _settingManager.GetOrNullAsync(LocalizationSettingNames.CurrentCulture);
            if (!string.IsNullOrEmpty(cultureFromGlobalSetting))
            {
                _logger.LogDebug("Culture resolved from global setting: {Culture}", cultureFromGlobalSetting);
                return cultureFromGlobalSetting;
            }
        }

        return null;
    }

    public string GetDefaultCulture()
    {
        return DefaultCultureName;
    }

    public List<string> GetCultureFallbackChain(string cultureName)
    {
        var chain = new List<string>();

        if (string.IsNullOrEmpty(cultureName))
        {
            chain.Add(GetDefaultCulture());
            return chain;
        }

        // Add the requested culture
        chain.Add(cultureName);

        // Add parent culture if it's a specific culture (e.g., fr-CA → fr)
        if (cultureName.Contains('-'))
        {
            var parentCulture = cultureName.Split('-')[0];
            if (!chain.Contains(parentCulture))
            {
                chain.Add(parentCulture);
            }
        }

        // Add default culture if not already in chain
        var defaultCulture = GetDefaultCulture();
        if (!chain.Contains(defaultCulture))
        {
            chain.Add(defaultCulture);
        }

        return chain;
    }

    private string? GetCultureFromQueryString()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            return null;

        return httpContext.Request.Query[CultureQueryStringKey].FirstOrDefault();
    }

    private string? GetCultureFromCookie()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            return null;

        var cookie = httpContext.Request.Cookies[CultureCookieKey];
        if (string.IsNullOrEmpty(cookie))
            return null;

        // Cookie format: c=en|uic=en
        // We extract the culture part (c=en)
        var parts = cookie.Split('|');
        foreach (var part in parts)
        {
            if (part.StartsWith("c="))
            {
                return part.Substring(2);
            }
        }

        return null;
    }
}
