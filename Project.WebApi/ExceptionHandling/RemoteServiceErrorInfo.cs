using System.Collections.Generic;

namespace Project.WebApi.ExceptionHandling;

public class RemoteServiceErrorInfo
{
    public string? Code { get; set; }
    public string Message { get; set; }
    public string? Details { get; set; }
    public string? Severity { get; set; }
    public Dictionary<string, object> Data { get; set; }
    public RemoteServiceValidationErrorInfo[]? ValidationErrors { get; set; }

    public RemoteServiceErrorInfo(string message, string? details = null, string? code = null)
    {
        Message = message;
        Details = details;
        Code = code;
        Data = new Dictionary<string, object>();
    }
}

public class RemoteServiceValidationErrorInfo
{
    public string? Message { get; set; }
    public string[]? Members { get; set; }

    public RemoteServiceValidationErrorInfo(string? message, string[]? members = null)
    {
        Message = message;
        Members = members;
    }
}

public class RemoteServiceErrorResponse
{
    public RemoteServiceErrorInfo Error { get; set; }

    public RemoteServiceErrorResponse(RemoteServiceErrorInfo error)
    {
        Error = error;
    }
}
