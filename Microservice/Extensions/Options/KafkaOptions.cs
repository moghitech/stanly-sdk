namespace Fusion.Microservice.Extensions.Options;

/// <summary>
/// Represents the configuration options for Kafka integration in the application.
/// </summary>
public class KafkaOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether Kafka integration is enabled.
    /// </summary>
    public bool IsEnabled { get; set; } = false;

    /// <summary>
    /// Gets or sets the address of the Kafka bootstrap server used for establishing the initial connection to the Kafka cluster.
    /// </summary>
    public string BootstrapServer { get; set; } = null!;

    /// <summary>
    /// Gets or sets the username used for authenticating with the Kafka server.
    /// </summary>
    public string Username { get; set; } = null!;

    /// <summary>
    /// Gets or sets the password used for authenticating with the Kafka broker.
    /// </summary>
    public string Password { get; set; } = null!;

    /// <summary>
    /// Gets or sets the identifier for the Kafka consumer group used for message consumption.
    /// </summary>
    public string GroupId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the number of messages to prefetch for processing from the Kafka topic.
    /// </summary>
    public ushort PrefetchCount { get; set; } = 100;

    /// <summary>
    /// Gets or sets the maximum number of concurrent operations allowed for Kafka message processing.
    /// </summary>
    public int ConcurrencyLimit { get; set; } = 10;

    /// <summary>
    /// Gets or sets the interval at which checkpoints are created during Kafka message processing.
    /// </summary>
    public int CheckpointInterval { get; set; } = 10;

    /// <summary>
    /// Gets or sets the number of messages to process before updating the checkpoint in Kafka integration.
    /// </summary>
    public int CheckpointMessageCount { get; set; } = 100;
}