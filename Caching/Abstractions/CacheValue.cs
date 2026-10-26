namespace Fusion.Caching.Abstractions;

/// <summary>
/// Represents a cached value with an indicator of its presence in the cache.
/// </summary>
/// <typeparam name="T">The type of the value stored in the cache.</typeparam>
/// <param name="HasValue">Indicates whether the cache contains a valid value.</param>
/// <param name="Value">The value stored in the cache, if it exists.</param>
public record CacheValue<T>(
    bool HasValue,
    T Value
);