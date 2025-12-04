using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Project.Domain.Interfaces;
using System.Reflection;

namespace Project.WebApi.Infrastructure;

/// <summary>
/// Discovers Application Services and exposes them as API controllers.
/// </summary>
public class DynamicApiControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
{
    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
    {
        // Search through all application parts for AppServices
        foreach (var part in parts.OfType<AssemblyPart>())
        {
            var appServiceTypes = part.Types
                .Where(t => typeof(IApplicationService).IsAssignableFrom(t) 
                    && t.IsClass 
                    && !t.IsAbstract 
                    && t.Name.EndsWith("AppService"));

            foreach (var type in appServiceTypes)
            {
                // Add to controller feature
                feature.Controllers.Add(type);
            }
        }
    }
}
