namespace Project.Core.Interfaces;

public interface IAuditContext
{
    string? ServiceName { get; set; }
    string? MethodName { get; set; }
    string? ClientIpAddress { get; set; }
    string? BrowserInfo { get; set; }
}
