using Fusion.Messaging.RabbitMq.Events;
using Fusion.Messaging.RabbitMq.Integration.Abstractions;
using MassTransit;
using IEvent = Fusion.Messaging.RabbitMq.Events.IEvent;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Fusion.Messaging.RabbitMq.Internals;

/// <inheritdoc cref="IEventPublisher"/>>
public class EventPublisher : IEventPublisher
{
    private readonly IBusControl _busControl;

    /// <summary> Constructor </summary>
    public EventPublisher(IBusControl busControl)
    {
        _busControl = busControl;
    }

    /// <inheritdoc cref="IEventPublisher.PublishAsync{TEvent}"/>>
    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
        where TEvent : class, IEvent
    {
        cancellationToken.ThrowIfCancellationRequested();

        await _busControl.Publish(@event, cancellationToken);
    }

    /// <inheritdoc cref="IEventPublisher.PublishWithDelayAsync{TEvent}"/>>
    public async Task PublishWithDelayAsync<TEvent>(TEvent @event, TimeSpan delay, CancellationToken cancellationToken)
        where TEvent : class, IEvent
    {
        cancellationToken.ThrowIfCancellationRequested();

        await _busControl.Publish(@event, context => { context.Delay = delay; }, cancellationToken);
    }

    /// <inheritdoc cref="IEventPublisher.PublishEntityEventAsync{TEvent}"/>>
    async Task IEventPublisher.PublishEntityEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (@event is null) return;

        var entityEvent = JsonSerializer.Serialize((object)@event);
        var eventTypeFullName = @event.GetType().AssemblyQualifiedName ?? @event.GetType().ToString();
        var baseEvent = new BaseEntityEvent { Event = entityEvent, EventTypeFullName = eventTypeFullName };

        await _busControl.Publish(baseEvent, cancellationToken);
    }
}