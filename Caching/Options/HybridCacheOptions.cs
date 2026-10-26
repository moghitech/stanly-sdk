namespace Fusion.Caching.Options;

/// <summary>
/// Provides configuration options for the hybrid cache mechanism.
/// </summary>
public class HybridCacheOptions
{
    /// <summary>
    /// Gets or sets the default duration for storing items in the memory cache when no specific expiration is defined.
    /// </summary>
    /// <remarks>
    /// This property specifies the default time span for which items will remain in the memory cache.
    /// If not explicitly set, the default value is 10 minutes.
    /// Modify this property to adjust the default cache duration according to application requirements.
    /// </remarks>
    public TimeSpan DefaultMemoryCacheDuration { get; set; } = TimeSpan.FromMinutes(10);
}