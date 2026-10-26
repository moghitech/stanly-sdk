using MassTransit;

namespace Fusion.Messaging.Kafka.Abstractions;

/// <summary>
/// Represents an abstraction for a Kafka messaging bus, providing mechanisms
/// to facilitate message publishing, consuming, and handling within a Kafka-based system.
/// </summary>
public interface IKafkaBus : IBus
{
}