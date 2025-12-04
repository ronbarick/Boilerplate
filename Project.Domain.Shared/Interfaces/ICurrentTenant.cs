using System;

namespace Project.Domain.Shared.Interfaces;

public interface ICurrentTenant
{
    Guid? Id { get; }
    string? Name { get; }
    bool IsAvailable { get; }
    IDisposable Change(Guid? tenantId, string? name = null);
}
