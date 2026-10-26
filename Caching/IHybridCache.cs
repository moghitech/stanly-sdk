using Fusion.Caching.Abstractions.Options;

namespace Fusion.Caching;

/// <summary>
/// Represents a hybrid caching mechanism that combines in-memory cache and distributed cache (e.g., Redis)
/// to provide a unified caching solution with scalability and performance benefits.
/// </summary>
/// <remarks>
/// The HybridCache uses multiple cache providers to ensure fast in-memory access while maintaining
/// consistency and availability in distributed environments. It also integrates with Redis to support
/// cache invalidation across multiple instances.
/// </remarks>
public interface IHybridCache
{
    /// <summary>
    /// Retrieves a cached value by the specified key or sets and caches a value if it does not exist, using the provided factory function and cache options.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve or set in the cache.</typeparam>
    /// <param name="key">The key associated with the cached value.</param>
    /// <param name="factory">A factory function to generate the value if it does not exist in the cache.</param>
    /// <param name="option">The caching options specifying expiration and other configurations.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the cached value or the newly generated value if set.</returns>
    Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, CacheOption option, CancellationToken ct);

    /// <summary>
    /// Retrieves a cached value by the specified key or generates and caches the value if it does not exist,
    /// using the provided factory function and cache duration.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve or set in the cache.</typeparam>
    /// <param name="key">The key associated with the cached value.</param>
    /// <param name="factory">A factory function to generate the value if it does not exist in the cache.</param>
    /// <param name="cacheDuration">The duration for which the value will be cached.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the cached value or the newly generated value if set.</returns>
    Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan cacheDuration, CancellationToken ct);

    /// <summary>
    /// Retrieves a cached value by the specified key. If the value does not exist, the task result will be default for the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve from the cache.</typeparam>
    /// <param name="key">The key associated with the cached value.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the cached value or the default value for the specified type if not found.</returns>
    Task<T> GetAsync<T>(string key, CancellationToken ct);

    /// <summary>
    /// Sets a value in the cache for the specified duration.
    /// Updates the cache in a hybrid mechanism involving in-memory and distributed cache layers.
    /// </summary>
    /// <typeparam name="T">The type of the value to set in the cache.</typeparam>
    /// <param name="key">The key under which the value should be stored in the cache.</param>
    /// <param name="value">The value to cache.</param>
    /// <param name="cacheDuration">The duration for which the value should remain in the cache.</param>
    /// <param name="ct">A cancellation token to observe while the task is being completed.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the cached value.</returns>
    Task SetAsync<T>(string key, T value, TimeSpan cacheDuration, CancellationToken ct);

    /// <summary>
    /// Sets a value in the cache identified by the specified key using the provided cache options.
    /// The value is stored for the duration and settings specified in the <see cref="CacheOption"/> parameter.
    /// </summary>
    /// <typeparam name="T">The type of the value to be cached.</typeparam>
    /// <param name="key">The key to associate with the cached value.</param>
    /// <param name="value">The value to cache.</param>
    /// <param name="option">The caching options, including expiration and serialization settings, for storing the value.</param>
    /// <param name="ct">A cancellation token to observe while performing the cache operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the value set in the cache.</returns>
    Task SetAsync<T>(string key, T value, CacheOption option, CancellationToken ct);

    /// <summary>
    /// Removes a cache entry with the specified key from both in-memory and Redis caches.
    /// Additionally, publishes a cache invalidation message to notify other instances.
    /// </summary>
    /// <param name="key">The key of the cache entry to be removed.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RemoveAsync(string key, CancellationToken ct);

    /// <summary>
    /// Determines whether a cached value exists for the specified key.
    /// </summary>
    /// <param name="key">The key associated with the cached value to check for existence.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is true if the cached value exists, otherwise false.</returns>
    Task<bool> ExistsAsync(string key, CancellationToken ct);

    /// <summary>
    /// Removes all cache entries with keys that start with the specified prefix from both in-memory and Redis caches.
    /// Additionally, publishes a cache invalidation message to notify other instances.
    /// </summary>
    /// <param name="prefix">The prefix to match against cache keys for removal.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RemoveByPrefixAsync(string prefix, CancellationToken ct);
}