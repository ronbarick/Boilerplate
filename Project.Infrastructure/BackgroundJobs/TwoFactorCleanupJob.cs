using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.BackgroundJobs;

/// <summary>
/// Background job to clean up expired two-factor codes.
/// Runs hourly via Hangfire.
/// </summary>
public class TwoFactorCleanupJob
{
    private readonly AppDbContext _context;
    private readonly ILogger<TwoFactorCleanupJob> _logger;

    public TwoFactorCleanupJob(AppDbContext context, ILogger<TwoFactorCleanupJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        try
        {
            var cutoffTime = DateTime.UtcNow.AddHours(-1);

            // Delete expired OTP codes
            var expiredCodes = await _context.TwoFactorCodes
                .Where(c => c.ExpiresAt < cutoffTime || c.IsUsed)
                .ToListAsync();

            if (expiredCodes.Any())
            {
                _context.TwoFactorCodes.RemoveRange(expiredCodes);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Cleaned up {Count} expired two-factor codes", expiredCodes.Count);
            }

            // Clean up expired trusted devices
            var expiredDevices = await _context.TrustedDevices
                .Where(d => d.ExpiresAt < DateTime.UtcNow && !d.IsDeleted)
                .ToListAsync();

            if (expiredDevices.Any())
            {
                foreach (var device in expiredDevices)
                {
                    device.IsDeleted = true;
                }
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Cleaned up {Count} expired trusted devices", expiredDevices.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up two-factor data");
            throw;
        }
    }
}
