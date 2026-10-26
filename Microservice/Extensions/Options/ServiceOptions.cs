namespace Fusion.Microservice.Extensions.Options;

/// <summary>
/// Represents the configuration options for the service.
/// This class contains properties to configure various subsystems,
/// such as OpenTelemetry, Sentry, Seq, and Authentication, for the service.
/// </summary>
public sealed class ServiceOptions
{
    /// <summary>
    /// Represents the name of the service.
    /// This property is used to uniquely identify the service in telemetry, logging, and other configurations.
    /// </summary>
    public string ServiceName { get; init; } = null!;

    /// <summary>
    /// Optional prefix used to identify or group backend services (e.g., "admin", "core").
    /// </summary>
    public string? BackendPrefix { get; set; }

    /// <summary>
    /// Indicates whether the current service is a backend service.
    /// </summary>
    public bool IsBackend { get; init; }

    /// <summary>
    /// Represents the RabbitMQ configuration options for the service.
    /// This property contains settings such as host, username, password, and port,
    /// required for establishing a connection to a RabbitMQ server.
    /// </summary>
    public RabbitMqOptions RabbitMQOptions { get; init; } = null!;

    /// <summary>
    /// Represents the configuration options for integrating OpenTelemetry
    /// into the service to enable distributed tracing and telemetry data collection.
    /// </summary>
    public OpenTelemetryOptions OpenTelemetry { get; set; } = null!;

    /// <summary>
    /// Represents the configuration options for integrating Sentry into the service
    /// to capture and report exceptions, logs, and performance data.
    /// </summary>
    public SentryOptions SentryOptions { get; set; } = null!;

    /// <summary>
    /// Provides configuration settings for integrating with a Seq server for logging
    /// and monitoring within the application.
    /// </summary>
    public SeqOptions SeqOptions { get; set; } = null!;

    /// <summary>
    /// Represents configuration options related to authentication.
    /// This property is used to configure and control authentication behavior
    /// within the service.
    /// </summary>
    public AuthenticationOptions AuthenticationOptions { get; set; } = null!;

    /// <summary>
    /// Defines configuration options required for connecting to a Redis server.
    /// </summary>
    public RedisOptions RedisOptions { get; set; } = null!;

    /// <summary>
    /// Provides configuration options for integrating Kafka into the application.
    /// This property encapsulates settings such as the Kafka server, authentication credentials,
    /// consumer group, prefetch behavior, concurrency limits, and checkpointing configuration.
    /// </summary>
    public KafkaOptions KafkaOptions { get; set; } = null!;
}