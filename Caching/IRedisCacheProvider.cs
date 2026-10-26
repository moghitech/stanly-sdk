using StackExchange.Redis;

namespace Fusion.Caching;

/// <summary>
/// Defines methods for working with Redis cache, extending the functionality of <see cref="ICacheProvider"/>.
/// </summary>
public interface IRedisCacheProvider : ICacheProvider
{
    /// <summary>
    /// Removes and returns the first element of a Redis list identified by the specified key.
    /// </summary>
    /// <param name="key">The key of the Redis list from which the first element should be removed and returned.</param>
    /// <param name="ct">A token to monitor for cancellation requests.</param>
    /// <typeparam name="T">The type of the value to be retrieved from the Redis list.</typeparam>
    /// <returns>A task that represents the asynchronous operation, containing the first element of the list or the default value if the list is empty or the key does not exist.</returns>
    Task<T> LPopAsync<T>(string key, CancellationToken ct);

    /// <summary>
    /// Appends a value to the end of a Redis list identified by the specified key.
    /// Optionally sets a time-to-live (TTL) for the key.
    /// </summary>
    /// <param name="key">The key of the Redis list to which the value should be appended.</param>
    /// <param name="value">The value to append to the Redis list.</param>
    /// <param name="ct">A token to monitor for cancellation requests.</param>
    /// <param name="ttl">An optional time-to-live value for the key, after which the key will expire.</param>
    /// <typeparam name="T">The type of the value to be appended to the Redis list.</typeparam>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RPushAsync<T>(string key, T value, CancellationToken ct, TimeSpan? ttl = null);

    /// <summary>
    /// Retrieves the remaining time-to-live (TTL) for a specified key in the Redis cache.
    /// </summary>
    /// <param name="key">The key for which the time-to-live (TTL) is to be retrieved.</param>
    /// <param name="ct">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation, containing the time span indicating the key's remaining TTL, or null if the key does not exist or has no associated expiration.</returns>
    Task<TimeSpan?> GetTimeToLiveAsync(string key, CancellationToken ct);

    /// <summary>
    /// Asynchronously sets multiple hash entries for the specified Redis hash key.
    /// </summary>
    /// <param name="key">The key of the Redis hash where the entries should be set.</param>
    /// <param name="entries">An array of hash entries to be set in the Redis hash.</param>
    /// <param name="flags">Optional command flags that specify the behavior of the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task HashMultipleSetAsync(string key, HashEntry[] entries, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Retrieves all hash fields and their values stored at the specified key in the Redis database.
    /// </summary>
    /// <param name="key">The key of the hash from which all fields and their values should be retrieved.</param>
    /// <param name="flags">Optional command flags that specify the behavior of the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing an array of <see cref="HashEntry"/> objects representing the fields and their values in the hash.</returns>
    Task<HashEntry[]> HashGetAllAsync(string key, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Retrieves the value associated with the specified field in a Redis hash identified by the given key.
    /// </summary>
    /// <param name="key">The key of the Redis hash from which the value should be retrieved.</param>
    /// <param name="field">The field within the hash whose value should be retrieved.</param>
    /// <param name="flags">The command flags to specify the behavior of the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the value associated with the specified field, or a null value if the field does not exist.</returns>
    Task<RedisValue> HashGetAsync(string key, string field, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Adds or updates the specified field in a hash stored at the provided key with the given value.
    /// </summary>
    /// <param name="key">The key of the Redis hash to be updated.</param>
    /// <param name="field">The field within the hash to be set or updated.</param>
    /// <param name="value">The value to assign to the specified field in the hash.</param>
    /// <param name="when">A condition that specifies whether to set the hash field depending on its existence.</param>
    /// <param name="flags">The command execution flags that specify pipeline or other operation characteristics.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task HashSetAsync(string key, string field, RedisValue value, When when, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Removes all cache entries matching the specified pattern from the Redis cache.
    /// </summary>
    /// <param name="pattern">The pattern used to match keys in the Redis cache for removal.</param>
    /// <param name="ct">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation of removing cache entries matching the specified pattern.</returns>
    Task RemoveByPatternAsync(string pattern, CancellationToken ct);

    /// <summary>
    /// Atomically increments the numeric value stored at the specified key in Redis by one.
    /// </summary>
    /// <param name="key">The key of the Redis cache entry whose value should be incremented.</param>
    /// <param name="ct">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation, containing the new incremented value.</returns>
    Task<long> IncrementAsync(string key, CancellationToken ct);

    /// <summary>
    /// Retrieves all cache keys matching the specified pattern from the Redis cache.
    /// </summary>
    /// <param name="pattern">The pattern used to match keys in the Redis cache.</param>
    /// <param name="ct">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation, containing a list of keys matching the pattern.</returns>
    Task<List<string>> GetKeysByPatternAsync(string pattern, CancellationToken ct);
}