using System;
using Microsoft.AspNetCore.Http;
using Project.Domain.Interfaces;

namespace Project.WebApi.Services;

public class CurrentTenant : ICurrentTenant
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentTenant(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? Id
    {
        get
        {
            var tenantId = _httpContextAccessor.HttpContext?.Items["TenantId"] as Guid?;
            return tenantId;
        }
    }

    public string? Name
    {
        get
        {
            return _httpContextAccessor.HttpContext?.Items["TenantName"] as string;
        }
    }

    public bool IsAvailable => Id.HasValue;

    public IDisposable Change(Guid? tenantId, string? name = null)
    {
        return new DisposeAction(() => { });
    }

    private class DisposeAction : IDisposable
    {
        private readonly Action _action;

        public DisposeAction(Action action)
        {
            _action = action;
        }

        public void Dispose()
        {
            _action();
        }
    }
}
