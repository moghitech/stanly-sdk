namespace Fusion.Messaging.RabbitMq.Handlers;

/// <summary>
/// Defines a strategy for retrieving the appropriate event handler based on the type of the event.
/// </summary>
public interface IEventHandlerStrategy
{
    /// <summary>
    /// Retrieves the event handler associated with the specified event type.
    /// </summary>
    /// <param name="eventType">The type of the event for which a handler is required.</param>
    /// <returns>An instance of <see cref="IEventHandler"/> capable of handling the specified event type.</returns>
    IEventHandler? GetHandler(string eventType);
}   