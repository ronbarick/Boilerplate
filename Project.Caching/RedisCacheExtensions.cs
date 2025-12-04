using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Project.Caching;

public static class RedisCacheExtensions
{
    public static IServiceCollection AddRedisDistributedCache(this IServiceCollection services, IConfiguration configuration)
    {
        var redisEnabled = configuration.GetValue<bool>("Redis:Enabled");

        if (redisEnabled)
        {
            var connectionString = configuration.GetConnectionString("Redis");
            
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Redis connection string is not configured.");
            }

            // Add Redis distributed cache
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = connectionString;
                options.InstanceName = configuration["Redis:InstanceName"] ?? "ProjectApp:";
            });

            // Add Redis connection multiplexer for advanced operations
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                return ConnectionMultiplexer.Connect(connectionString);
            });

            // Add Redis output cache
            services.AddStackExchangeRedisOutputCache(options =>
            {
                options.Configuration = connectionString;
                options.InstanceName = configuration["Redis:InstanceName"] ?? "ProjectApp:";
            });
        }
        else
        {
            // Fallback to in-memory cache
            services.AddDistributedMemoryCache();
            
            services.AddOutputCache(options =>
            {
                options.AddBasePolicy(builder => builder.Expire(TimeSpan.FromSeconds(10)));
            });
        }

        // Register cache services
        services.AddScoped<IDistributedCacheService, DistributedCacheService>();
        services.AddScoped<ICacheInvalidationService, CacheInvalidationService>();

        return services;
    }
}
