namespace Fusion.Caching.Constants;

/// <summary>
/// Provides constant values for Redis channel usage throughout the application.
/// </summary>
public static class RedisChannelConstants
{
    /// <summary>
    /// Represents the Redis channel name used for hybrid cache invalidation in the caching system.
    /// This constant is typically used to facilitate communication across distributed cache providers,
    /// enabling cache invalidation and synchronization mechanisms.
    /// </summary>
    public const string HybridCacheInvalidation = "hybrid-cache-invalidation";
}