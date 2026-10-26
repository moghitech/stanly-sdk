namespace Fusion.Messaging.RabbitMq.Integration.Abstractions;

/// <summary>
/// Represents a generic event wrapper for publishing and consuming cross-assembly events.
/// </summary>
public class BaseEntityEvent
{
    /// <summary>
    /// Assembly-qualified name of the concrete event type.
    /// </summary>
    public string EventTypeFullName { get; init; } = null!;

    /// <summary>
    /// The event serialized as JSON.
    /// </summary>
    public string Event { get; init; } = null!;
}