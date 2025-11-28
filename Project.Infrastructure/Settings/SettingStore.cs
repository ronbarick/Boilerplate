using Microsoft.EntityFrameworkCore;
using Project.Core.Entities;
using Project.Core.Interfaces;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Settings;

/// <summary>
/// EF Core implementation of ISettingStore.
/// </summary>
public class SettingStore : ISettingStore
{
    private readonly AppDbContext _dbContext;

    public SettingStore(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string?> GetOrNullAsync(string name, string providerName, string? providerKey)
    {
        var setting = await _dbContext.Settings
            .Where(s => s.Name == name && s.ProviderName == providerName && s.ProviderKey == providerKey)
            .FirstOrDefaultAsync();

        return setting?.Value;
    }

    public async Task SetAsync(string name, string value, string providerName, string? providerKey)
    {
        var setting = await _dbContext.Settings
            .Where(s => s.Name == name && s.ProviderName == providerName && s.ProviderKey == providerKey)
            .FirstOrDefaultAsync();

        if (setting == null)
        {
            setting = new Setting
            {
                Name = name,
                Value = value,
                ProviderName = providerName,
                ProviderKey = providerKey
            };
            _dbContext.Settings.Add(setting);
        }
        else
        {
            setting.Value = value;
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(string name, string providerName, string? providerKey)
    {
        var setting = await _dbContext.Settings
            .Where(s => s.Name == name && s.ProviderName == providerName && s.ProviderKey == providerKey)
            .FirstOrDefaultAsync();

        if (setting != null)
        {
            _dbContext.Settings.Remove(setting);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<Dictionary<string, string>> GetAllAsync(string providerName, string? providerKey)
    {
        var settings = await _dbContext.Settings
            .Where(s => s.ProviderName == providerName && s.ProviderKey == providerKey)
            .ToListAsync();

        return settings.ToDictionary(s => s.Name, s => s.Value ?? string.Empty);
    }
}
