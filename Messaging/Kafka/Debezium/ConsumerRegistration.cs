namespace Fusion.Messaging.Kafka.Debezium;

/// <summary>
/// Represents a record used to register a Kafka consumer, its associated event type,
/// and the topic wildcard pattern for matching topics that the consumer should process.
/// </summary>
/// <param name="ConsumerType">
/// The type of the consumer that processes messages. This type should implement an interface
/// for message consumption, such as <see cref="MassTransit.IConsumer{T}" />.
/// </param>
/// <param name="EventType">
/// The type of the event that the consumer processes. This defines the type of message
/// payload the consumer expects to handle.
/// </param>
/// <param name="TopicWildcard">
/// A string defining the wildcard pattern for Kafka topics. Topics matching this pattern
/// will be consumed by the specified consumer.
/// </param>
/// <param name="HandlerType">
/// Optional. The type of the handler that implements <see cref="Handlers.ICdcEventHandler{T}"/>.
/// This is used when the consumer is a wrapper around a custom handler.
/// </param>
public record ConsumerRegistration(
    Type ConsumerType, 
    Type EventType, 
    string TopicWildcard, 
    Type? HandlerType = null);