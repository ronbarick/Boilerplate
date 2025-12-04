using Microsoft.AspNetCore.OutputCaching;
using StackExchange.Redis;

namespace Project.Caching;

public interface ICacheInvalidationService
{
    Task InvalidatePermissionCacheAsync(Guid? userId = null);
    Task InvalidateFeatureCacheAsync(Guid? tenantId = null);
    Task InvalidateSettingCacheAsync(Guid? userId = null, Guid? tenantId = null);
    Task InvalidateOutputCacheAsync(string tag);
    Task InvalidateAllAsync();
}

public class CacheInvalidationService : ICacheInvalidationService
{
    private readonly IDistributedCacheService _cache;
    private readonly IOutputCacheStore _outputCacheStore;
    private readonly IConnectionMultiplexer? _redis;

    public CacheInvalidationService(
        IDistributedCacheService cache,
        IOutputCacheStore outputCacheStore,
        IConnectionMultiplexer? redis = null)
    {
        _cache = cache;
        _outputCacheStore = outputCacheStore;
        _redis = redis;
    }

    public async Task InvalidatePermissionCacheAsync(Guid? userId = null)
    {
        if (userId.HasValue)
        {
            await _cache.RemoveAsync($"auth:permissions:user:{userId}");
        }
        else
        {
            await RemoveByPatternAsync("auth:permissions:*");
        }

        await InvalidateOutputCacheAsync("app-config");
    }

    public async Task InvalidateFeatureCacheAsync(Guid? tenantId = null)
    {
        if (tenantId.HasValue)
        {
            await _cache.RemoveAsync($"features:tenant:{tenantId}");
        }
        else
        {
            await RemoveByPatternAsync("features:*");
        }

        await InvalidateOutputCacheAsync("app-config");
    }

    public async Task InvalidateSettingCacheAsync(Guid? userId = null, Guid? tenantId = null)
    {
        if (userId.HasValue)
        {
            await _cache.RemoveAsync($"settings:user:{userId}");
        }
        else if (tenantId.HasValue)
        {
            await _cache.RemoveAsync($"settings:tenant:{tenantId}");
        }
        else
        {
            await RemoveByPatternAsync("settings:*");
        }

        await InvalidateOutputCacheAsync("app-config");
    }

    public async Task InvalidateOutputCacheAsync(string tag)
    {
        await _outputCacheStore.EvictByTagAsync(tag, CancellationToken.None);
    }

    public async Task InvalidateAllAsync()
    {
        await RemoveByPatternAsync("*");
        await _outputCacheStore.EvictByTagAsync("app-config", CancellationToken.None);
        await _outputCacheStore.EvictByTagAsync("localization", CancellationToken.None);
    }

    private async Task RemoveByPatternAsync(string pattern)
    {
        if (_redis == null)
            return;

        var server = _redis.GetServer(_redis.GetEndPoints().First());
        var keys = server.Keys(pattern: pattern);

        foreach (var key in keys)
        {
            await _redis.GetDatabase().KeyDeleteAsync(key);
        }
    }
}
