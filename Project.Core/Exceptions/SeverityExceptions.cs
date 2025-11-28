using System;

namespace Project.Core.Exceptions;

public class InternalException : SeverityExceptionBase
{
    public InternalException(string message, string? code = null, string? details = null, Exception? innerException = null)
        : base(SeverityLevel.Internal, message, code, details, innerException)
    {
    }
}

public class ErrorException : SeverityExceptionBase
{
    public ErrorException(string message, string? code = null, string? details = null, Exception? innerException = null)
        : base(SeverityLevel.Error, message, code, details, innerException)
    {
    }
}

public class WarningException : SeverityExceptionBase
{
    public WarningException(string message, string? code = null, string? details = null, Exception? innerException = null)
        : base(SeverityLevel.Warning, message, code, details, innerException)
    {
    }
}

public class InfoException : SeverityExceptionBase
{
    public InfoException(string message, string? code = null, string? details = null, Exception? innerException = null)
        : base(SeverityLevel.Info, message, code, details, innerException)
    {
    }
}
