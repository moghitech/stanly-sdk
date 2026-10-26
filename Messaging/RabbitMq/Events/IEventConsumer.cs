using MassTransit;

namespace Fusion.Messaging.RabbitMq.Events;

/// <summary>
/// Defines a contract for handling messages in a messaging system.
/// </summary>
public interface IEventConsumer<in TEvent> : IConsumer<TEvent> where TEvent : class, IEvent;