using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;

namespace Project.WebApi.Helpers;

/// <summary>
/// Helper to generate device fingerprints from IP address and User-Agent.
/// </summary>
public static class DeviceFingerprintHelper
{
    public static string GenerateFingerprint(HttpContext context)
    {
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var userAgent = context.Request.Headers["User-Agent"].ToString();

        var combined = $"{ipAddress}|{userAgent}";
        
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
        return Convert.ToBase64String(hashBytes);
    }

    public static string GetDeviceName(HttpContext context)
    {
        var userAgent = context.Request.Headers["User-Agent"].ToString();
        
        // Simple device name extraction (can be enhanced)
        if (userAgent.Contains("Windows"))
            return "Windows Device";
        if (userAgent.Contains("Mac"))
            return "Mac Device";
        if (userAgent.Contains("Linux"))
            return "Linux Device";
        if (userAgent.Contains("Android"))
            return "Android Device";
        if (userAgent.Contains("iPhone") || userAgent.Contains("iPad"))
            return "iOS Device";
        
        return "Unknown Device";
    }
}
