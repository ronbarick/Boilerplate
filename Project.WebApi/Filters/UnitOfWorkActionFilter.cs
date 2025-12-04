using Microsoft.AspNetCore.Mvc.Filters;
using Project.Domain.UnitOfWork;

namespace Project.WebApi.Filters;

public class UnitOfWorkActionFilter : IAsyncActionFilter
{
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public UnitOfWorkActionFilter(IUnitOfWorkManager unitOfWorkManager)
    {
        _unitOfWorkManager = unitOfWorkManager;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var uowAttribute = context.ActionDescriptor.EndpointMetadata
            .OfType<UnitOfWorkAttribute>()
            .FirstOrDefault();

        if (uowAttribute == null || uowAttribute.IsDisabled)
        {
            await next();
            return;
        }

        using (var uow = _unitOfWorkManager.Begin(isTransactional: uowAttribute.IsTransactional))
        {
            var result = await next();
            if (result.Exception == null || result.ExceptionHandled)
            {
                await uow.CommitAsync();
            }
        }
    }
}
