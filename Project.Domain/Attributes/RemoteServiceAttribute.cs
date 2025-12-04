using System;

namespace Project.Domain.Attributes;

/// <summary>
/// Attribute to control whether a service or method should be exposed as a remote API endpoint.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method)]
public class RemoteServiceAttribute : Attribute
{
    public bool IsEnabled { get; }

    public RemoteServiceAttribute(bool isEnabled = true)
    {
        IsEnabled = isEnabled;
    }
}
