using Fusion.Messaging.RabbitMq.Integration.Abstractions;

namespace Fusion.Messaging.RabbitMq.Handlers;

/// <summary>
/// Represents a general event handler that processes events and supports asynchronous operations.
/// </summary>
public interface IEventHandler
{
    /// <summary>
    /// Handles a given entity event asynchronously.
    /// </summary>
    /// <param name="event">The entity event to be handled.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous handling operation.</returns>
    Task HandleAsync(IEntityEvent @event, CancellationToken cancellationToken);
}

/// <inheritdoc cref="IEventHandler"/>
public interface IEventHandler<in T> where T : IEntityEvent
{
    /// <inheritdoc cref="IEventHandler.HandleAsync"/>
    Task HandleAsync(T @event, CancellationToken cancellationToken);
}