using Fusion.Messaging.RabbitMq.Events;

namespace Fusion.Messaging.RabbitMq.Integration.Abstractions;

/// <summary>
/// Represents a message used in the messaging infrastructure.
/// This interface is intended to serve as a marker for types
/// that are used within the messaging pipeline, enabling
/// message handling and configuration.
/// </summary>
public interface IEntityEvent : IEvent
{
    /// <summary>
    /// Gets the timestamp indicating when the event occurred.
    /// This property provides the date and time information
    /// associated with the specific entity event.
    /// </summary>
    DateTimeOffset? Timestamp { get; set; }
}