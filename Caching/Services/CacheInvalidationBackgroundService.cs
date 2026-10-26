using Fusion.Caching.Abstractions.Messages;
using Fusion.Caching.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Fusion.Caching.Services;

/// <summary>
/// Represents a background service responsible for handling cache invalidation in memory cache
/// triggered by messages published in a Redis channel.
/// </summary>
/// <remarks>
/// The <see cref="CacheInvalidationBackgroundService"/> subscribes to a Redis channel and listens
/// for cache invalidation messages. Upon receiving a message, it removes the corresponding cache
/// entry from the configured memory cache to ensure consistency across distributed nodes in the system.
/// </remarks>
public sealed class CacheInvalidationBackgroundService(
    IConnectionMultiplexer connectionMultiplexer,
    IConfiguration configuration,
    IMemoryCache memoryCache,
    ILogger<CacheInvalidationBackgroundService> logger
) : BackgroundService
{
    /// <summary>
    /// Executes the cache invalidation background service logic.
    /// </summary>
    /// <param name="stoppingToken">The <see cref="CancellationToken"/> used to observe service shutdown signals.</param>
    /// <returns>An asynchronous operation.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var channelName = RedisChannelHelpers.CacheInvalidation(configuration);
            var channel = RedisChannel.Literal(channelName);

            var subscriber = connectionMultiplexer.GetSubscriber();
            await subscriber.SubscribeAsync(channel, (_, message) =>
            {
                var cacheInvalidateMessage = CacheInvalidateMessage.FromJson(message!);
                memoryCache.Remove(cacheInvalidateMessage!.Key);
            });
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occured while executing the cache invalidation");
            SentrySdk.CaptureException(e);
        }
    }
}