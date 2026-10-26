using Fusion.Caching.Abstractions;
using Fusion.Caching.Abstractions.Enums;
using Fusion.Caching.Abstractions.Options;
using Microsoft.Extensions.Caching.Memory;

namespace Fusion.Caching.Internals;

internal class MemoryCacheProvider(IMemoryCache database) : ICacheProvider
{
    public CacheProviderType ProviderType => CacheProviderType.InMemory;

    public Task<T> GetAsync<T>(string key, CancellationToken ct)
    {
        var value = database.Get<T>(key);
        return Task.FromResult(value!);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken ct)
    {
        database.Set(key, value, expiration);
        return Task.CompletedTask;
    }

    public Task<bool> RemoveAsync(string key, CancellationToken ct)
    {
        database.Remove(key);
        return Task.FromResult(true);
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken ct)
    {
        var result = await GetAsync<object?>(key, ct);
        return result is not null;
    }

    public async Task<T> GetOrSetAsync<T>(string cacheKey, Func<Task<T>> getDataFunc, CacheOption option,
        CancellationToken ct)
    {
        return await GetOrSetAsync(cacheKey, getDataFunc, option.Expiration, ct);
    }

    public async Task SetAsync<T>(string key, T value, CacheOption option, CancellationToken ct)
    {
        await SetAsync(key, value, option.Expiration, ct);
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
        catch (Exception)
        {
            return await getDataFunc();
        }
    }

    public Task<CacheValue<T>> TryGetValueAsync<T>(string key, CancellationToken ct)
    {
        var cachedValue = database.Get<T>(key);

        return Task.FromResult(cachedValue is not null
            ? new CacheValue<T>(true, cachedValue)
            : new CacheValue<T>(false, default!)
        );
    }
}