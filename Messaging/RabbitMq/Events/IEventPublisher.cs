using Fusion.Messaging.RabbitMq.Integration.Abstractions;

namespace Fusion.Messaging.RabbitMq.Events;

/// <summary>
/// Defines an interface for a message publisher that provides functionality
/// to publish messages asynchronously and optionally with a delay.
/// </summary>
public interface IEventPublisher
{
    /// <summary>
    /// Publishes the specified event asynchronously using the messaging infrastructure.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event being published. Must implement <see cref="IEvent"/>.</typeparam>
    /// <param name="event">The event instance to be published.</param>
    /// <param name="cancellationToken">A token used to propagate notification that the operation should be canceled.</param>
    /// <returns>A task representing the asynchronous publish operation.</returns>
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : class, IEvent;

    /// <summary>
    /// Publishes the specified event asynchronously with a specified delay to the messaging infrastructure.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event being published. Must implement <see cref="IEvent"/>.</typeparam>
    /// <param name="event">The event instance to be published.</param>
    /// <param name="delay">The time span to delay before publishing the event.</param>
    /// <param name="cancellationToken">A token to monitor for request cancellation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task PublishWithDelayAsync<TEvent>(TEvent @event, TimeSpan delay, CancellationToken cancellationToken)
        where TEvent : class, IEvent;

    /// <summary>
    /// Publishes the specified entity event asynchronously using the messaging infrastructure.
    /// </summary>
    /// <param name="event">The entity event instance to be published. Must implement <see cref="IEntityEvent"/>.</param>
    /// <param name="cancellationToken">A token used to propagate notification that the operation should be canceled.</param>
    /// <returns>A task representing the asynchronous publish operation.</returns>
    internal Task PublishEntityEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken);
}