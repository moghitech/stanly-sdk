using Fusion.Messaging.Kafka.Abstractions;
using Fusion.Messaging.Kafka.Events;
using Fusion.Messaging.Kafka.Handlers;
using Fusion.Messaging.Kafka.Helpers;

namespace Fusion.Messaging.Kafka.Debezium.Masstransit;

/// <inheritdoc cref="IKafkaHandlerConfigurator"/>
public class KafkaHandlerConfigurator : IKafkaHandlerConfigurator
{
    /// <summary>
    /// Gets the collection of consumer registrations that defines the consumer types, event types,
    /// and topic wildcards to be used for handling Kafka messages.
    /// </summary>
    /// <remarks>
    /// Each registration in the collection specifies the mapping between a consumer type, an event type,
    /// and a topic wildcard, enabling the configuration of message consumption in a Kafka system.
    /// The registrations are used during the Kafka consumer setup to define how topics are consumed
    /// and processed by specific consumer implementations.
    /// </remarks>
    public List<ConsumerRegistration> Registrations { get; } = [];

    /// <inheritdoc cref="IKafkaHandlerConfigurator.AddHandler{THandler,TEvent}"/>
    public void AddHandler<THandler, TEvent>()
        where THandler : class, ICdcEventHandler<TEvent>
        where TEvent : class, ICdcEvent
    {
        var legacyName = CdcEventTypeMapper.GetLegacyEntityName(typeof(TEvent));

        var topicWildcard = CdcTopicWildcardBuilder.ForLegacyEntity(legacyName!);

        var consumerType = typeof(CdcEventConsumer<TEvent>);

        Registrations.Add(new ConsumerRegistration(consumerType, typeof(TEvent), topicWildcard, typeof(THandler)));
    }
}