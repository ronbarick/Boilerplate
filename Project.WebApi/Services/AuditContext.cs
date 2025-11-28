using Project.Core.Interfaces;

namespace Project.WebApi.Services;

public class AuditContext : IAuditContext
{
    public string? ServiceName { get; set; }
    public string? MethodName { get; set; }
    public string? ClientIpAddress { get; set; }
    public string? BrowserInfo { get; set; }
}
