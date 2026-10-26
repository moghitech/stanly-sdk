using Fusion.Messaging.RabbitMq.Integration.Abstractions;

namespace Fusion.Messaging.RabbitMq.Integration.Helpers;

/// <summary>
/// Provides helper methods for working with entity-based integration events.
/// </summary>
public static class EntityEventHelpers
{
    /// <summary>
    /// Converts a <see cref="BaseEntityEvent"/> into its strongly typed <see cref="IEntityEvent"/> instance
    /// by resolving the event type from its full name and deserializing the stored event payload.
    /// </summary>
    /// <param name="event">
    /// The base event wrapper containing the event type full name and serialized event payload.
    /// </param>
    /// <returns>
    /// A deserialized <see cref="IEntityEvent"/> instance if the type is resolved and deserialization succeeds;
    /// otherwise <c>null</c>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the event type specified in <see cref="BaseEntityEvent.EventTypeFullName"/> cannot be found.
    /// </exception>
    public static IEntityEvent? ToEntityEvent(this BaseEntityEvent @event)
    {
        var eventType = Type.GetType(@event.EventTypeFullName);

        if (eventType is null)
            throw new InvalidOperationException($"Type {@event.EventTypeFullName} not found");

        return (IEntityEvent?)System.Text.Json.JsonSerializer.Deserialize(@event.Event, eventType);
    }
}