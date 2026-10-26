namespace Fusion.Messaging.Kafka.Events;

/// <summary>
/// Represents a generic event interface that can be implemented
/// to define events within the messaging framework.
/// </summary>
public interface ICdcEvent
{
    /// <summary>
    /// Gets or sets the unique identifier for the customer associated with the event.
    /// </summary>
    public long CustomerId { get; set; }
}