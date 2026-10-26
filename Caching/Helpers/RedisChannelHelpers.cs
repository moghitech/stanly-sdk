using Fusion.Caching.Constants;
using Fusion.Microservice.Extensions;
using Microsoft.Extensions.Configuration;

namespace Fusion.Caching.Helpers;

internal static class RedisChannelHelpers
{
    internal static string CacheInvalidation(IConfiguration configuration)
    {
        return $"{configuration.GetServiceOptions().ServiceName}:{RedisChannelConstants.HybridCacheInvalidation}";
    }
}