namespace Fusion.Storage.Configuration;

/// <summary>
/// The object storage settings object model.
/// </summary>
internal class ObjectStorageSettings
{
    /// <summary>
    /// Key for reading from configuration.
    /// </summary>
    public const string ObjectStorageSettingsConfiguration = "ObjectStorageSettings";

    /// <summary>
    /// The object storage end point url. 
    /// </summary>
    public string EndPoint { get; init; } = string.Empty;

    /// <summary>
    /// The access key to use the object storage.
    /// </summary>
    public string AccessKey { get; init; } = string.Empty;

    /// <summary>
    /// The secret key to use the object storage.
    /// </summary>
    public string SecretKey { get; init; } = string.Empty;

    /// <summary>
    /// The bucket name of object storage.
    /// </summary>
    public string BucketName { get; init; } = string.Empty;

    /// <summary>
    /// The settings for use ssl or not in object storage.
    /// </summary>
    public bool WithSsl { get; init; }
}