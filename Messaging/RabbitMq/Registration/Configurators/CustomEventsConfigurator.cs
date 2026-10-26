using Fusion.Messaging.RabbitMq.Abstractions;
using Fusion.Messaging.RabbitMq.Events;
using Fusion.Messaging.RabbitMq.Integration.Configuration;

namespace Fusion.Messaging.RabbitMq.Registration.Configurators;

/// <summary>
/// Configurator for custom domain events and their consumers.
/// </summary>
public class CustomEventsConfigurator
{
    internal readonly List<Type> ConsumerTypes = [];
    internal readonly List<Action<IMessagingRegistrationContext, IBusFactoryConfigurator>> ConfigActions = [];

    /// <summary>
    /// Adds a custom event with its consumer and queue.
    /// </summary>
    public void Add<TEvent, TConsumer>(string queueName)
        where TConsumer : class, IEventConsumer<TEvent> where TEvent : class, IEvent
    {
        ConsumerTypes.Add(typeof(TConsumer));
        ConfigActions.Add((ctx, cfg) => { cfg.AddCustomEvent<TEvent, TConsumer>(ctx, queueName); });
    }
}