using Fusion.Messaging.RabbitMq.Integration.Abstractions;

namespace Fusion.Messaging.RabbitMq.Handlers;

internal sealed class EventHandlerWrapper<TEvent>(IEventHandler<TEvent> innerHandler)
    : IEventHandler where TEvent : IEntityEvent
{
    public async Task HandleAsync(IEntityEvent @event, CancellationToken cancellationToken)
    {
        if (@event is not TEvent specificEvent)
        {
            throw new ArgumentException($"Handler expects {nameof(TEvent)}.");
        }

        await innerHandler.HandleAsync(specificEvent, cancellationToken);
    }
}