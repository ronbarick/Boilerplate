using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Project.Caching;

public interface IDistributedCacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default);
}

public class DistributedCacheService : IDistributedCacheService
{
    private readonly IDistributedCache _cache;

    public DistributedCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var data = await _cache.GetStringAsync(key, cancellationToken);
        
        if (string.IsNullOrEmpty(data))
            return default;

        return JsonSerializer.Deserialize<T>(data);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var options = new DistributedCacheEntryOptions();
        
        if (expiration.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = expiration.Value;
        }
        else
        {
            options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10); // Default 10 minutes
        }

        var json = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(key, json, options, cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(key, cancellationToken);
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        // Note: This requires Redis-specific implementation
        // For now, this is a placeholder that doesn't do anything
        // In production, you would use StackExchange.Redis directly for pattern-based deletion
        await Task.CompletedTask;
    }
}
