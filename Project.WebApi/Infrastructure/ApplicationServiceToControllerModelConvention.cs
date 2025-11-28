using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Project.Core.Interfaces;
using Project.WebApi.Filters;

namespace Project.WebApi.Infrastructure;

/// <summary>
/// Configures routing conventions for Application Services.
/// Converts AppService methods to RESTful API endpoints following ABP conventions.
/// </summary>
public class ApplicationServiceToControllerModelConvention : IApplicationModelConvention
{
    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            var type = controller.ControllerType;

            // Only process Application Services
            if (!typeof(IApplicationService).IsAssignableFrom(type))
                continue;

            // Configure controller
            ConfigureController(controller);
            
            // Apply [Authorize] filter to all AppService controllers
            controller.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter());
            
            // Apply AutoValidateFilter to enforce model validation
            controller.Filters.Add(new AutoValidateFilter());
        }
    }

    private void ConfigureController(ControllerModel controller)
    {
        // Remove "AppService" suffix from controller name
        var controllerName = controller.ControllerName.Replace("AppService", "");
        
        // Set route: /api/app/{serviceName}
        controller.Selectors.Clear();
        controller.Selectors.Add(new SelectorModel
        {
            AttributeRouteModel = new AttributeRouteModel(new RouteAttribute($"api/app/{controllerName.ToLowerInvariant()}"))
        });

        // Configure actions
        var actionsToRemove = new List<ActionModel>();
        foreach (var action in controller.Actions)
        {
            // Check for [RemoteService(false)]
            var remoteServiceAttr = action.ActionMethod.GetCustomAttributes(typeof(Project.Core.Attributes.RemoteServiceAttribute), true)
                .FirstOrDefault() as Project.Core.Attributes.RemoteServiceAttribute;

            if (remoteServiceAttr != null && !remoteServiceAttr.IsEnabled)
            {
                actionsToRemove.Add(action);
                continue;
            }

            ConfigureAction(action);
            
            // Apply permission filters from RequiresPermissionAttribute
            ApplyPermissionFilters(action);
        }

        // Remove excluded actions
        foreach (var action in actionsToRemove)
        {
            controller.Actions.Remove(action);
        }
    }

    private void ApplyPermissionFilters(ActionModel action)
    {
        // Check for RequiresPermissionAttribute on the action method
        var permissionAttributes = action.ActionMethod
            .GetCustomAttributes(typeof(Project.Core.Attributes.RequiresPermissionAttribute), inherit: true)
            .Cast<Project.Core.Attributes.RequiresPermissionAttribute>()
            .ToList();

        // Also check for RequiresPermissionAttribute on the controller class
        permissionAttributes.AddRange(
            action.Controller.ControllerType
                .GetCustomAttributes(typeof(Project.Core.Attributes.RequiresPermissionAttribute), inherit: true)
                .Cast<Project.Core.Attributes.RequiresPermissionAttribute>()
        );

        // Apply a filter for each permission attribute
        foreach (var attr in permissionAttributes)
        {
            action.Filters.Add(new Project.WebApi.Filters.PermissionAuthorizationFilterFactory(attr));
        }
    }

    private void ConfigureAction(ActionModel action)
    {
        var methodName = action.ActionName;

        // Clear existing selectors
        action.Selectors.Clear();

        // Determine HTTP method and route based on method name
        if (methodName.StartsWith("Get"))
        {
            if (methodName == "GetListAsync" || methodName == "GetList")
            {
                // GET /api/app/{service}
                action.Selectors.Add(new SelectorModel
                {
                    AttributeRouteModel = new AttributeRouteModel(),
                    ActionConstraints = { new HttpMethodActionConstraint(new[] { "GET" }) }
                });
            }
            else if (methodName == "GetAsync" || methodName == "Get")
            {
                // GET /api/app/{service}/{id}
                action.Selectors.Add(new SelectorModel
                {
                    AttributeRouteModel = new AttributeRouteModel(new RouteAttribute("{id}")),
                    ActionConstraints = { new HttpMethodActionConstraint(new[] { "GET" }) }
                });
            }
            else
            {
                // GET /api/app/{service}/{action}
                action.Selectors.Add(new SelectorModel
                {
                    AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(methodName.Replace("Async", "").ToLowerInvariant())),
                    ActionConstraints = { new HttpMethodActionConstraint(new[] { "GET" }) }
                });
            }
        }
        else if (methodName.StartsWith("Create"))
        {
            // POST /api/app/{service}
            action.Selectors.Add(new SelectorModel
            {
                AttributeRouteModel = new AttributeRouteModel(),
                ActionConstraints = { new HttpMethodActionConstraint(new[] { "POST" }) }
            });
            
            // Add [FromBody] for complex type parameters (DTOs)
            ConfigureBodyParameter(action);
        }
        else if (methodName.StartsWith("Update"))
        {
            // PUT /api/app/{service}/{id}
            action.Selectors.Add(new SelectorModel
            {
                AttributeRouteModel = new AttributeRouteModel(new RouteAttribute("{id}")),
                ActionConstraints = { new HttpMethodActionConstraint(new[] { "PUT" }) }
            });
            
            // Add [FromBody] for complex type parameters (DTOs)
            ConfigureBodyParameter(action);
        }
        else if (methodName.StartsWith("Delete"))
        {
            // DELETE /api/app/{service}/{id}
            action.Selectors.Add(new SelectorModel
            {
                AttributeRouteModel = new AttributeRouteModel(new RouteAttribute("{id}")),
                ActionConstraints = { new HttpMethodActionConstraint(new[] { "DELETE" }) }
            });
        }
        else
        {
            // Default: POST for other methods
            action.Selectors.Add(new SelectorModel
            {
                AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(methodName.ToLowerInvariant())),
                ActionConstraints = { new HttpMethodActionConstraint(new[] { "POST" }) }
            });
            
            // Add [FromBody] for complex type parameters (DTOs)
            ConfigureBodyParameter(action);
        }
    }

    private void ConfigureBodyParameter(ActionModel action)
    {
        // Find complex type parameters (DTOs) and mark them as [FromBody]
        foreach (var parameter in action.Parameters)
        {
            // Skip simple types (Guid, int, string, etc.) and mark complex types as [FromBody]
            if (!parameter.ParameterType.IsPrimitive && 
                parameter.ParameterType != typeof(string) && 
                parameter.ParameterType != typeof(Guid) &&
                parameter.ParameterType != typeof(DateTime) &&
                !parameter.ParameterType.IsEnum)
            {
                parameter.BindingInfo = parameter.BindingInfo ?? new Microsoft.AspNetCore.Mvc.ModelBinding.BindingInfo();
                parameter.BindingInfo.BindingSource = Microsoft.AspNetCore.Mvc.ModelBinding.BindingSource.Body;
            }
        }
    }
}
