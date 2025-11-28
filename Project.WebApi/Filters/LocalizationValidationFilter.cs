using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Project.Core.Localization;

namespace Project.WebApi.Filters;

/// <summary>
/// Asynchronous action filter to handle model validation and localize error messages.
/// Replaces the default ModelStateInvalidFilter.
/// </summary>
public class LocalizationValidationFilter : IAsyncActionFilter
{
    private readonly ILocalizationManager _localizationManager;

    public LocalizationValidationFilter(ILocalizationManager localizationManager)
    {
        _localizationManager = localizationManager;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = new Dictionary<string, string[]>();

            foreach (var key in context.ModelState.Keys)
            {
                var modelStateEntry = context.ModelState[key];
                if (modelStateEntry != null && modelStateEntry.Errors.Any())
                {
                    var localizedErrors = new List<string>();

                    foreach (var error in modelStateEntry.Errors)
                    {
                        var errorMessage = error.ErrorMessage;

                        // Try to localize the error message (assuming it's a key)
                        // We use the default Project resource
                        var localizedMessage = await _localizationManager.GetStringAsync(
                            ProjectLocalizationResource.ResourceName,
                            errorMessage);

                        localizedErrors.Add(localizedMessage);
                    }

                    errors.Add(key, localizedErrors.ToArray());
                }
            }

            var problemDetails = new ValidationProblemDetails(errors)
            {
                Status = 400,
                Title = await _localizationManager.GetStringAsync(ProjectLocalizationResource.ResourceName, "Common:ValidationFailed") 
                        ?? "One or more validation errors occurred."
            };

            context.Result = new BadRequestObjectResult(problemDetails);
            return;
        }

        await next();
    }
}
