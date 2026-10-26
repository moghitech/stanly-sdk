using Fusion.Caching.Abstractions.Enums;
using Fusion.Caching.Abstractions.Messages;
using Fusion.Caching.Abstractions.Options;
using Fusion.Caching.Helpers;
using Fusion.Caching.Options;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Fusion.Caching.Internals;

/// <inheritdoc cref="IHybridCache"/>
internal sealed class HybridCache : IDisposable, IHybridCache
{
    private readonly IEnumerable<ICacheProvider> _cacheProviders;
    private readonly IConnectionMultiplexer _redisConnection;
    private readonly string _channelName;
    private readonly TimeSpan _defaultMemoryCacheDuration;

    /// <inheritdoc cref="IHybridCache"/>
    public HybridCache(
        IEnumerable<ICacheProvider> cacheProviders,
        IConnectionMultiplexer redisConnection,
        IConfiguration configuration,
        IOptions<HybridCacheOptions> hybridCacheOptions
    )
    {
        _cacheProviders = cacheProviders;
        _redisConnection = redisConnection;
        _channelName = RedisChannelHelpers.CacheInvalidation(configuration);
        _defaultMemoryCacheDuration = hybridCacheOptions.Value.DefaultMemoryCacheDuration;
    }

    /// <inheritdoc cref="IHybridCache.GetOrSetAsync{T}(string,System.Func{System.Threading.Tasks.Task{T}},Fusion.Caching.Abstractions.Options.CacheOption,System.Threading.CancellationToken)"/>
    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, CacheOption option,
        CancellationToken ct)
    {
        var memoryCache = GetCacheProvider(CacheProviderType.InMemory);
        var memoryCacheResult = await memoryCache.GetAsync<T>(key, ct);
        if (memoryCacheResult is not null && !EqualityComparer<T>.Default.Equals(memoryCacheResult, default))
            return memoryCacheResult;

        var redisCache = GetCacheProvider(CacheProviderType.Redis);
        var redisCacheResult = await redisCache.GetAsync<T>(key, ct);
        if (redisCacheResult is not null && !EqualityComparer<T>.Default.Equals(redisCacheResult, default))
        {
            await memoryCache.SetAsync(key, redisCacheResult, option with { Expiration = _defaultMemoryCacheDuration },
                ct);
            return redisCacheResult;
        }

        await RemoveAsync(key, ct);
        var result = await factory();

        await memoryCache.SetAsync(key, result, option with { Expiration = _defaultMemoryCacheDuration },
            ct);
        await redisCache.SetAsync(key, result, option, ct);

        return result;
    }

    /// <inheritdoc cref="IHybridCache.GetOrSetAsync{T}(string,System.Func{System.Threading.Tasks.Task{T}},System.TimeSpan,System.Threading.CancellationToken)"/>
    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan cacheDuration,
        CancellationToken ct)
    {
        var memoryCache = GetCacheProvider(CacheProviderType.InMemory);
        var memoryCacheResult = await memoryCache.GetAsync<T>(key, ct);
        if (memoryCacheResult is not null && !EqualityComparer<T>.Default.Equals(memoryCacheResult, default))
            return memoryCacheResult;

        var redisCache = GetCacheProvider(CacheProviderType.Redis);
        var redisCacheResult = await redisCache.GetAsync<T>(key, ct);
        if (redisCacheResult is not null && !EqualityComparer<T>.Default.Equals(redisCacheResult, default))
        {
            await memoryCache.SetAsync(key, redisCacheResult, _defaultMemoryCacheDuration, ct);
            return redisCacheResult;
        }

        await RemoveAsync(key, ct);
        var result = await factory();

        await memoryCache.SetAsync(key, result, _defaultMemoryCacheDuration, ct);
        await redisCache.SetAsync(key, result, cacheDuration, ct);

        return result;
    }

    /// <inheritdoc cref="IHybridCache.GetAsync{T}"/>
    public async Task<T> GetAsync<T>(string key, CancellationToken ct)
    {
        var memoryCache = GetCacheProvider(CacheProviderType.InMemory);
        var memoryCacheResult = await memoryCache.GetAsync<T>(key, ct);
        if (memoryCacheResult is not null && !EqualityComparer<T>.Default.Equals(memoryCacheResult, default))
            return memoryCacheResult;

        var redisCache = GetCacheProvider(CacheProviderType.Redis);
        var redisCacheResult = await redisCache.GetAsync<T>(key, ct);
        return redisCacheResult is not null ? redisCacheResult : default!;
    }

    /// <inheritdoc cref="IHybridCache.SetAsync{T}(string,T,System.TimeSpan,System.Threading.CancellationToken)"/>
    public async Task SetAsync<T>(string key, T value, TimeSpan cacheDuration, CancellationToken ct)
    {
        await RemoveAsync(key, ct);

        var memoryCache = GetCacheProvider(CacheProviderType.InMemory);
        var redisCache = GetCacheProvider(CacheProviderType.Redis);

        await memoryCache.SetAsync(key, value, _defaultMemoryCacheDuration, ct);
        await redisCache.SetAsync(key, value, cacheDuration, ct);
    }

    /// <inheritdoc cref="IHybridCache.SetAsync{T}(string,T,Fusion.Caching.Abstractions.Options.CacheOption,System.Threading.CancellationToken)"/>
    public async Task SetAsync<T>(string key, T value, CacheOption option, CancellationToken ct)
    {
        await RemoveAsync(key, ct);

        var memoryCache = GetCacheProvider(CacheProviderType.InMemory);
        var redisCache = GetCacheProvider(CacheProviderType.Redis);

        await memoryCache.SetAsync(key, value, option with { Expiration = _defaultMemoryCacheDuration },
            ct);
        await redisCache.SetAsync(key, value, option, ct);
    }

    /// <inheritdoc cref="IHybridCache.RemoveAsync"/>
    public async Task RemoveAsync(string key, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        
        var memoryCache = GetCacheProvider(CacheProviderType.InMemory);
        var redisCache = GetCacheProvider(CacheProviderType.Redis);

        await memoryCache.RemoveAsync(key, ct);
        await redisCache.RemoveAsync(key, ct);

        var subscriber = _redisConnection.GetSubscriber();
        var message = new CacheInvalidateMessage(key, DateTime.UtcNow).ToJson();
        await subscriber.PublishAsync(RedisChannel.Literal(_channelName), new RedisValue(message));
    }

    /// <inheritdoc cref="IHybridCache.ExistsAsync"/>
    public async Task<bool> ExistsAsync(string key, CancellationToken ct)
    {
        var redisCache = GetCacheProvider(CacheProviderType.Redis);

        var existence = await redisCache.ExistsAsync(key, ct);

        return existence;
    }

    /// <inheritdoc cref="IHybridCache.RemoveByPrefixAsync"/>
    public async Task RemoveByPrefixAsync(string prefix, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        
        if (GetCacheProvider(CacheProviderType.Redis) is IRedisCacheProvider redisCache)
        {
            var pattern = $"{prefix}*";
            var keys = await redisCache.GetKeysByPatternAsync(pattern, ct);

            await Parallel.ForEachAsync(keys, ct, async (key, cancellationToken) =>
            {
                await RemoveAsync(key, cancellationToken);
            });
        }
    }

    public void Dispose()
    {
        var subscriber = _redisConnection.GetSubscriber();
        subscriber.UnsubscribeAll();
    }

    private ICacheProvider GetCacheProvider(CacheProviderType type)
    {
        return _cacheProviders.First(p => p.ProviderType == type);
    }
}