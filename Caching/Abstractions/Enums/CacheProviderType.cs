namespace Fusion.Caching.Abstractions.Enums;

/// <summary>
/// Represents the types of cache providers supported in the system.
/// </summary>
public enum CacheProviderType : byte
{
    /// <summary>
    /// Represents a Redis-based caching mechanism within the system.
    /// This cache type utilizes a distributed, in-memory data store,
    /// providing scalability, data persistence, and high availability.
    /// Suitable for scenarios requiring shared state management across multiple instances or services.
    /// </summary>
    Redis = 0,

    /// <summary>
    /// Represents an in-memory caching mechanism within the system.
    /// This cache type stores data temporarily in the application's memory space,
    /// providing fast access to cached data. Suitable for scenarios where
    /// low latency and simplicity are priorities, and distributed cache is not required.
    /// </summary>
    InMemory = 1
}