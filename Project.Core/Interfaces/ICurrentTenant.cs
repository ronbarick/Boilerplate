using System;

namespace Project.Core.Interfaces;

public interface ICurrentTenant
{
    Guid? Id { get; }
    string? Name { get; }
    bool IsAvailable { get; }
}
