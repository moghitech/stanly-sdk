namespace Fusion.Caching.Abstractions.Options;

/// <summary>
/// Represents options for configuring caching serialization behavior.
/// </summary>
public record struct CacheOption
{
    /// <summary>
    /// Gets or sets a value indicating whether to ignore self-referencing loops
    /// during serialization. When enabled, this option prevents circular reference
    /// errors that may occur due to objects referencing themselves directly or indirectly.
    /// </summary>
    public bool SelfReferenceIgnorance { get; set; }

    /// <summary>
    /// Gets or sets the duration of time after which a cached item expires.
    /// Once the expiration time is reached, the cached item is no longer valid
    /// and will be removed or refreshed according to the caching policy.
    /// </summary>
    public TimeSpan Expiration { get; set; }
}