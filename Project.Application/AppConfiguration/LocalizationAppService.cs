using Project.Application.AppConfiguration.Dtos;
using Project.Application.Services;
using Project.Domain.Interfaces;
using Project.Domain.Localization;

namespace Project.Application.AppConfiguration;

public class LocalizationAppService : AppServiceBase, ILocalizationAppService
{
    private readonly ILocalizationProvider _localizationProvider;

    public LocalizationAppService(
        ICurrentUser currentUser,
        ICurrentTenant currentTenant,
        IPermissionChecker permissionChecker,
        ILocalizationManager localizationManager,
        ILocalizationProvider localizationProvider)
        : base(currentUser, currentTenant, permissionChecker, localizationManager)
    {
        _localizationProvider = localizationProvider;
    }

    public async Task<LocalizationTextsDto> GetTextsAsync(string culture, string? resourceName = null)
    {
        resourceName ??= "Project"; // Default resource

        // Get all strings for the resource
        var dictionary = await _localizationProvider.LoadDictionaryAsync(resourceName, culture);
        var texts = dictionary ?? new Dictionary<string, string>();

        return new LocalizationTextsDto
        {
            Culture = culture,
            ResourceName = resourceName,
            Texts = texts
        };
    }
}
