namespace Fusion.Messaging.Kafka.Handlers;

/// <summary>
/// Defines a contract for handling Change Data Capture (CDC) events.
/// This interface allows target projects to implement CDC event handling logic
/// without any direct dependency on MassTransit or other messaging infrastructure.
/// </summary>
/// <typeparam name="TEvent">The type of CDC event to be handled.</typeparam>
public interface ICdcEventHandler<in TEvent> where TEvent : class
{
    /// <summary>
    /// Handles the specified CDC event asynchronously.
    /// </summary>
    /// <param name="event">The CDC event to be processed.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}