using Fusion.Caching;
using Fusion.Caching.Internals;
using Microsoft.Extensions.DependencyInjection;

namespace Fusion.Microservice.Extensions.ORM;

/// <summary>
/// Memory Cache Registration
/// </summary>
public static class MemoryCacheRegistration
{
    /// <summary>
    /// Register In-Memory Cache
    /// </summary>
    /// <param name="services"></param>
    public static void AddInMemoryCache(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<ICacheProvider, MemoryCacheProvider>();
    }
}