using Fusion.Messaging.Kafka.Events;
using Fusion.Messaging.Kafka.Handlers;

namespace Fusion.Messaging.Kafka.Abstractions;

/// <summary>
/// Defines a configuration interface for setting up Kafka consumers within an application,
/// allowing the registration of consumers and their associated event types tied to specific
/// topic patterns.
/// </summary>
public interface IKafkaHandlerConfigurator
{
    /// <summary>
    /// Registers a CDC event handler for processing messages from topics that match
    /// the pattern derived from the event's legacy entity name.
    /// This method provides a decoupled way to handle CDC events without direct MassTransit dependencies.
    /// </summary>
    /// <typeparam name="THandler">
    /// The type of the handler that implements <see cref="ICdcEventHandler{TEvent}"/>.
    /// </typeparam>
    /// <typeparam name="TEvent">
    /// The type of the CDC event that the handler processes.
    /// </typeparam>
    void AddHandler<THandler, TEvent>()
        where THandler : class, ICdcEventHandler<TEvent> where TEvent : class, ICdcEvent;
}