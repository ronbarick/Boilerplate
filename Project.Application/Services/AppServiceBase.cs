using System;
using System.Threading.Tasks;
using Project.Core.Interfaces;
using Project.Core.Localization;

namespace Project.Application.Services;

public abstract class AppServiceBase
{
    protected ICurrentUser CurrentUser { get; }
    protected ICurrentTenant CurrentTenant { get; }
    protected IPermissionChecker PermissionChecker { get; }
    protected ILocalizationManager LocalizationManager { get; }

    protected AppServiceBase(
        ICurrentUser currentUser,
        ICurrentTenant currentTenant,
        IPermissionChecker permissionChecker,
        ILocalizationManager localizationManager)
    {
        CurrentUser = currentUser;
        CurrentTenant = currentTenant;
        PermissionChecker = permissionChecker;
        LocalizationManager = localizationManager;
    }

    protected Task<string> L(string key, params object[] args)
    {
        return LocalizationManager.GetStringAsync(Project.Core.Localization.ProjectLocalizationResource.ResourceName, key, args);
    }

    protected async Task CheckPermissionAsync(string permissionName)
    {
        if (!await PermissionChecker.IsGrantedAsync(permissionName))
        {
            throw new UnauthorizedAccessException($"Required permission '{permissionName}' is missing.");
        }
    }
}
