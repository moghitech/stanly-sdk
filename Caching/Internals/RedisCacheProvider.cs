using Fusion.Caching.Abstractions;
using Fusion.Caching.Abstractions.Enums;
using Fusion.Caching.Abstractions.Options;
using Fusion.Microservice.Extensions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Fusion.Caching.Internals;

internal class RedisCacheProvider(IConnectionMultiplexer connectionMultiplexer, IConfiguration configuration)
    : IRedisCacheProvider
{
    private readonly IDatabase _database = connectionMultiplexer.GetDatabase();
    public CacheProviderType ProviderType => CacheProviderType.Redis;

    public async Task<T> GetAsync<T>(string key, CancellationToken ct)
    {
        var serializedData = await _database.StringGetAsync(GetFullKey(key));
        return !serializedData.IsNull ? JsonConvert.DeserializeObject<T>(serializedData!) ?? default! : default!;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken ct)
    {
        var serializedData = JsonConvert.SerializeObject(value);
        await _database.StringSetAsync(GetFullKey(key), serializedData, expiration);
    }

    public async Task<bool> RemoveAsync(string key, CancellationToken ct)
    {
        return await _database.KeyDeleteAsync(GetFullKey(key));
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken ct)
    {
        return await _database.KeyExistsAsync(GetFullKey(key));
    }

    public async Task<T> LPopAsync<T>(string key, CancellationToken ct)
    {
        var serializedData = await _database.ListLeftPopAsync(GetFullKey(key));

        return !serializedData.IsNull ? JsonConvert.DeserializeObject<T>(serializedData!) ?? default! : default!;
    }

    public async Task RPushAsync<T>(string key, T value, CancellationToken ct, TimeSpan? ttl = null)
    {
        var serializedData = JsonConvert.SerializeObject(value);
        var fullKey = GetFullKey(key);

        try
        {
            await _database.ListRightPushAsync(fullKey, [serializedData]);
        }
        catch (RedisServerException exception)
        {
            SentrySdk.CaptureException(exception);
            await _database.KeyDeleteAsync(fullKey);
            await _database.ListRightPushAsync(fullKey, [serializedData]);
        }

        if (ttl.HasValue)
        {
            await _database.KeyExpireAsync(fullKey, ttl.Value);
        }
    }

    public async Task<T> GetOrSetAsync<T>(string cacheKey, Func<Task<T>> getDataFunc, CacheOption option,
        CancellationToken ct)
    {
        try
        {
            var keyExist = await ExistsAsync(cacheKey, ct);
            if (keyExist)
                return await GetAsync<T>(cacheKey, ct);

            var cachedData = await getDataFunc();
            if (cachedData is not null)
                await SetAsync(cacheKey, cachedData, option, ct);

            return cachedData;
        }
        catch (RedisException)
        {
            return await getDataFunc();
        }
    }

    public async Task SetAsync<T>(string key, T value, CacheOption option, CancellationToken ct)
    {
        if (!option.SelfReferenceIgnorance)
            await SetAsync(key, value, option.Expiration, ct);

        var serializedData = JsonConvert.SerializeObject(value, settings: new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });
        await _database.StringSetAsync(GetFullKey(key), serializedData, option.Expiration);
    }

    public async Task<CacheValue<T>> TryGetValueAsync<T>(string key, CancellationToken ct)
    {
        var fullKey = GetFullKey(key);
        var exists = await ExistsAsync(key, ct);
        if (!exists)
            return new CacheValue<T>(false, default!);

        var value = await GetAsync<T>(fullKey, ct);
        return new CacheValue<T>(true, value!);
    }

    public async Task<T> GetOrSetAsync<T>(string cacheKey, Func<Task<T>> getDataFunc, TimeSpan cacheDuration,
        CancellationToken ct)
    {
        try
        {
            var keyExist = await ExistsAsync(cacheKey, ct);
            if (keyExist)
            {
                return await GetAsync<T>(cacheKey, ct);
            }

            var cachedData = await getDataFunc();
            if (cachedData != null)
            {
                await SetAsync(cacheKey, cachedData, cacheDuration, ct);
            }

            return cachedData;
        }
        catch (RedisException)
        {
            return await getDataFunc();
        }
    }

    public async Task<TimeSpan?> GetTimeToLiveAsync(string key, CancellationToken ct)
    {
        return await _database.KeyTimeToLiveAsync(GetFullKey(key));
    }

    public async Task HashMultipleSetAsync(string key, HashEntry[] entries, CommandFlags flags = CommandFlags.None)
    {
        await _database.HashSetAsync(GetFullKey(key), entries, flags);
    }

    public async Task HashSetAsync(string key, string field, RedisValue value, When when,
        CommandFlags flags = CommandFlags.None)
    {
        await _database.HashSetAsync(GetFullKey(key), field, value, when, flags);
    }

    public async Task<HashEntry[]> HashGetAllAsync(string key, CommandFlags flags = CommandFlags.None)
    {
        return await _database.HashGetAllAsync(GetFullKey(key), flags);
    }

    public async Task<RedisValue> HashGetAsync(string key, string field, CommandFlags flags = CommandFlags.None)
    {
        return await _database.HashGetAsync(GetFullKey(key), field, flags);
    }

    public async Task RemoveByPatternAsync(string pattern, CancellationToken ct)
    {
        var server = connectionMultiplexer.GetServer(connectionMultiplexer.GetEndPoints().First());

        var retrievedKeys = server.Keys(pattern: GetFullKey(pattern)).ToArray();

        await Parallel.ForEachAsync(retrievedKeys, cancellationToken: ct,
            async (key, _) => { await _database.KeyDeleteAsync(key); });
    }

    public async Task<long> IncrementAsync(string key, CancellationToken ct)
    {
        return await _database.StringIncrementAsync(GetFullKey(key));
    }

    public async Task<List<string>> GetKeysByPatternAsync(string pattern, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        
        var server = connectionMultiplexer.GetServer(connectionMultiplexer.GetEndPoints().First());
        var serviceName = configuration.GetServiceOptions().ServiceName;
        var servicePrefix = $"{serviceName}:";

        var retrievedKeys = new List<string>();
        await foreach (var key in server.KeysAsync(pattern: GetFullKey(pattern)).WithCancellation(ct))
            retrievedKeys.Add(key.ToString()[servicePrefix.Length..]);

        return retrievedKeys;
    }

    private string GetFullKey(string key)
    {
        var serviceName = configuration.GetServiceOptions().ServiceName;
        return $"{serviceName}:{key}";
    }
}