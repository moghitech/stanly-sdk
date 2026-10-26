using Fusion.Caching.Abstractions;
using Fusion.Caching.Abstractions.Enums;
using Fusion.Caching.Abstractions.Options;

namespace Fusion.Caching;

/// <summary>
/// Represents a general-purpose cache provider that supports retrieving, storing, and managing cached data.
/// </summary>
public interface ICacheProvider
{
    /// <summary>
    /// Gets the type of the cache provider. This property indicates whether the cache provider
    /// implementation is based on technologies such as InMemory or Redis.
    /// </summary>
    /// <value>
    /// A <see cref="CacheProviderType"/> that specifies the type of the cache provider.
    /// </value>
    CacheProviderType ProviderType { get; }

    /// <summary>
    /// Retrieves a value from the cache if it exists; otherwise, executes the provided function to obtain the value, stores it in the cache, and returns it.
    /// </summary>
    /// <param name="cacheKey">The key of the cache entry to retrieve or set.</param>
    /// <param name="getDataFunc">The function to execute to obtain the data if it is not present in the cache.</param>
    /// <param name="cacheDuration">The duration for which the value should remain in the cache.</param>
    /// <param name="ct">A cancellation token to cancel the operation.</param>
    /// <typeparam name="T">The type of the value to retrieve or set in the cache.</typeparam>
    /// <returns>A task that represents the asynchronous operation. The task result contains the value retrieved from the cache or obtained from the function.</returns>
    Task<T> GetOrSetAsync<T>(string cacheKey, Func<Task<T>> getDataFunc, TimeSpan cacheDuration,
        CancellationToken ct);

    /// <summary>
    /// Retrieves a value from the cache associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the cache entry to retrieve.</param>
    /// <param name="ct">A cancellation token to cancel the operation.</param>
    /// <typeparam name="T">The type of the value to retrieve from the cache.</typeparam>
    /// <returns>A task that represents the asynchronous operation. The task result contains the retrieved value, or the default value for the type if the key does not exist.</returns>
    Task<T> GetAsync<T>(string key, CancellationToken ct);

    /// <summary>
    /// Stores a value in the cache with the specified key and expiration duration.
    /// </summary>
    /// <param name="key">The key under which the value is stored in the cache.</param>
    /// <param name="value">The value to store in the cache.</param>
    /// <param name="expiration">The duration after which the cache entry should expire.</param>
    /// <param name="ct">A cancellation token to cancel the operation.</param>
    /// <typeparam name="T">The type of the value to store in the cache.</typeparam>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken ct);

    /// <summary>
    /// Removes the specified cache entry.
    /// </summary>
    /// <param name="key">The key of the cache entry to remove.</param>
    /// <param name="ct">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the removal was successful.</returns>
    Task<bool> RemoveAsync(string key, CancellationToken ct);

    /// <summary>
    /// Checks whether a cache entry with the specified key exists in the cache.
    /// </summary>
    /// <param name="key">The key of the cache entry to check for existence.</param>
    /// <param name="ct">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the cache entry exists.</returns>
    Task<bool> ExistsAsync(string key, CancellationToken ct);
    
    /// <summary>
    /// Retrieves a cached value identified by the given cache key, or sets it using the provided function if not present, with the specified cache options.
    /// </summary>
    /// <param name="cacheKey">The key associated with the cached value.</param>
    /// <param name="getDataFunc">A function to retrieve the value if it is not already cached.</param>
    /// <param name="option">The caching options to be applied, such as expiration and serialization configuration.</param>
    /// <param name="ct">A token to monitor for cancellation requests.</param>
    /// <typeparam name="T">The type of the value to be retrieved or set.</typeparam>
    /// <returns>A task that represents the asynchronous operation, containing the retrieved or newly cached value.</returns>
    Task<T> GetOrSetAsync<T>(string cacheKey, Func<Task<T>> getDataFunc, CacheOption option,
        CancellationToken ct);

    /// <summary>
    /// Asynchronously sets a value in the Redis cache with the specified key and configuration options.
    /// </summary>
    /// <param name="key">The key under which the value should be cached.</param>
    /// <param name="value">The value to be stored in the cache.</param>
    /// <param name="option">The caching options that determine behaviors such as expiration and self-reference handling.</param>
    /// <param name="ct">A token to monitor for cancellation requests.</param>
    /// <typeparam name="T">The type of the value to be stored in the cache.</typeparam>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SetAsync<T>(string key, T value, CacheOption option, CancellationToken ct);

    /// <summary>
    /// Attempts to retrieve a value from the cache associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the cache entry to retrieve.</param>
    /// <param name="ct">A cancellation token to cancel the operation.</param>
    /// <typeparam name="T">The type of the value contained in the cache.</typeparam>
    /// <returns>A task that represents the asynchronous operation. The task result contains a CacheValue object indicating whether the value exists in the cache and the associated value if found.</returns>
    Task<CacheValue<T>> TryGetValueAsync<T>(string key, CancellationToken ct);
}
