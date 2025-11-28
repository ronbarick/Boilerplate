using System;

namespace Project.Core.Exceptions;

public class UserFriendlyException : Exception
{
    public string? Code { get; set; }
    public string? Details { get; set; }

    public UserFriendlyException(string message, string? code = null, string? details = null, Exception? innerException = null)
        : base(message, innerException)
    {
        Code = code;
        Details = details;
    }
}
