using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Project.Domain.Interfaces;

namespace Project.WebApi.Filters;

public class AuditActionFilter : IActionFilter
{
    private readonly IAuditContext _auditContext;

    public AuditActionFilter(IAuditContext auditContext)
    {
        _auditContext = auditContext;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ActionDescriptor is ControllerActionDescriptor descriptor)
        {
            _auditContext.ServiceName = descriptor.ControllerName;
            _auditContext.MethodName = descriptor.ActionName;
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // No action needed after execution
    }
}
