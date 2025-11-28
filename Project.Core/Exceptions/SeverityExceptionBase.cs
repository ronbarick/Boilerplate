using System;

namespace Project.Core.Exceptions;

public abstract class SeverityExceptionBase : UserFriendlyException
{
    public SeverityLevel Severity { get; }

    protected SeverityExceptionBase(
        SeverityLevel severity,
        string message,
        string? code = null,
        string? details = null,
        Exception? innerException = null)
        : base(message, code, details, innerException)
    {
        Severity = severity;
        Data["Severity"] = severity;
    }
}
