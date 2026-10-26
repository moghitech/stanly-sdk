using Microsoft.EntityFrameworkCore;

namespace Fusion.Microservice.Extensions.Options;

/// <summary>
/// Represents configuration options for a PostgreSQL database connection.
/// </summary>
public class NpgOptions
{
    /// <summary>
    /// Gets or sets the connection string used for establishing a connection to the PostgreSQL database.
    /// </summary>
    /// <remarks>
    /// The connection string contains information about the database location, authentication credentials, and other parameters required for connectivity.
    /// Ensure the format and values are correct as per PostgreSQL connection string standards.
    /// </remarks>
    public string ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the query tracking behavior to be applied by the database context.
    /// </summary>
    /// <remarks>
    /// This property determines how the change tracker tracks entities in a query result.
    /// It can affect performance and memory usage, depending on the chosen tracking mode.
    /// Use options such as `TrackAll` for default tracking or `NoTracking` for read-only queries.
    /// </remarks>
    public QueryTrackingBehavior TrackingBehavior { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether lazy loading of related data is enabled.
    /// </summary>
    /// <remarks>
    /// When enabled, related data is automatically loaded from the database when it is accessed for the first time.
    /// If disabled, related data must be explicitly loaded using eager or explicit loading mechanisms.
    /// </remarks>
    public bool LazyLoading { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of retry attempts allowed when a database operation fails.
    /// </summary>
    /// <remarks>
    /// This property defines the number of times the system will attempt to retry a failed database operation
    /// before considering it as failed. It can be useful for handling transient issues with database connections
    /// or other recoverable errors.
    /// </remarks>
    public int MaxRetryOnFailureCount { get; set; }

    /// <summary>
    /// Gets or sets the maximum delay, in seconds, between retry attempts on failure
    /// for a PostgreSQL database connection.
    /// </summary>
    public int MaxRetryOnFailureDelayBySeconds { get; set; }

    /// <summary>
    /// Gets or sets the timeout value, in seconds, for a database command execution.
    /// </summary>
    /// <remarks>
    /// This property specifies the maximum time to wait for a command to execute before the operation is terminated.
    /// It is used to configure the command timeout setting for PostgreSQL database interactions.
    /// </remarks>
    public int CommandTimeout { get; set; }
}