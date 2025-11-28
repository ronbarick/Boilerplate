using System;
using Project.Core.Exceptions;

namespace Project.WebApi.ExceptionHandling;

public class SeverityErrorInfoConverter : IExceptionToErrorInfoConverter
{
    public RemoteServiceErrorInfo Convert(Exception exception, bool includeSensitiveDetails)
    {
        var errorInfo = CreateErrorInfoWithoutCode(exception, includeSensitiveDetails);

        if (exception is UserFriendlyException userFriendlyException)
        {
            errorInfo.Code = userFriendlyException.Code;
            errorInfo.Details = userFriendlyException.Details;
        }

        // Handle Severity
        var severity = SeverityLevel.Internal; // Default for unknown exceptions

        if (exception.Data.Contains("Severity") && exception.Data["Severity"] is SeverityLevel level)
        {
            severity = level;
        }
        else if (exception is UserFriendlyException)
        {
            severity = SeverityLevel.Error; // Default for UserFriendlyException
        }

        errorInfo.Severity = severity.ToString();
        errorInfo.Data["severity"] = severity.ToString(); // Keep in Data as well for backward compatibility if needed
        
        return errorInfo;
    }

    private RemoteServiceErrorInfo CreateErrorInfoWithoutCode(Exception exception, bool includeSensitiveDetails)
    {
        if (includeSensitiveDetails)
        {
            return new RemoteServiceErrorInfo(
                exception.Message,
                exception.ToString());
        }

        if (exception is UserFriendlyException)
        {
            return new RemoteServiceErrorInfo(exception.Message);
        }

        return new RemoteServiceErrorInfo("An internal error occurred during your request!");
    }
}
