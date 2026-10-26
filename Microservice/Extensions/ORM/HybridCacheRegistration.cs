using Fusion.Caching;
using Fusion.Caching.Internals;
using Fusion.Caching.Options;
using Fusion.Caching.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fusion.Microservice.Extensions.ORM;

/// <summary>
/// Hybrid Cache Registration
/// </summary>
public static class HybridCacheRegistration
{
    /// <summary>
    /// Registers and configures the hybrid caching mechanism, which combines in-memory and distributed cache services.
    /// This includes options for invalidation synchronization across nodes and centralized configuration for cache settings.
    /// </summary>
    /// <param name="services">The service collection instance used to register caching services.</param>
    /// <param name="configuration">Configuration instance to access application settings, including cache settings.</param>
    /// <param name="hybridCacheOptions">Optional custom configuration for the hybrid cache behavior.</param>
    /// <returns>A task that represents the asynchronous operation of adding the hybrid cache services.</returns>
    public static async Task AddHybridCache(this IServiceCollection services, IConfiguration configuration,
        HybridCacheOptions? hybridCacheOptions = null)
    {
        await services.AddRedisCache(configuration);
        services.AddInMemoryCache();

        services.Configure<HybridCacheOptions>(options =>
        {
            if (hybridCacheOptions is not null)
            {
                options.DefaultMemoryCacheDuration = hybridCacheOptions.DefaultMemoryCacheDuration;
            }
        });

        services.AddSingleton<IHybridCache, HybridCache>();

        /*
         * Cache Invalidation Background Service
         * ---------------------------------------
         * This background service is responsible for ensuring cache consistency across distributed
         * application nodes by handling cache invalidation in the in-memory cache.
         *
         * It achieves this by subscribing to a specific Redis channel that broadcasts invalidation messages.
         * When a message is received, the service decodes the message to determine the corresponding cache key
         * and removes that entry from the local memory cache.
         *
         * This mechanism ensures that stale data is not served to end users by synchronizing cache state across
         * different nodes. Additionally, the service logs any errors encountered during its operation, and it
         * captures these exceptions using an external monitoring tool for further analysis.
         */
        services.AddHostedService<CacheInvalidationBackgroundService>();
    }
}