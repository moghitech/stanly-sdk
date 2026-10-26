namespace Fusion.Microservice.Extensions.Options;

/// <summary>
/// Provides configuration options for connecting to a Redis server.
/// </summary>
public class RedisOptions
{
    /// <summary>
    /// Gets or sets the connection string used to connect to the Redis server.
    /// The connection string specifies the details necessary for establishing a
    /// connection, such as the server's address and port.
    /// for the sentinel configuration each connection string should separate with ','
    /// </summary>
    public required string ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the password used for authenticating connections to the Redis server.
    /// This is required if the Redis server has password-based authentication enabled.
    /// </summary>
    public required string Password { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the Redis setup uses sentinel mode.
    /// Sentinel mode is typically used for high availability, where it provides
    /// automatic fail-over by monitoring Redis master and replicas.
    /// </summary>
    public bool IsSentinel { get; set; }

    /// <summary>
    /// Gets or sets the name of the Redis service used in a Sentinel configuration.
    /// The service name is utilized to identify the master node within a Redis Sentinel setup.
    /// </summary>
    public string ServiceName { get; set; }
}