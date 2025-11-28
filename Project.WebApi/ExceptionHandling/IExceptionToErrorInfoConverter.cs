using System;

namespace Project.WebApi.ExceptionHandling;

public interface IExceptionToErrorInfoConverter
{
    RemoteServiceErrorInfo Convert(Exception exception, bool includeSensitiveDetails);
}
