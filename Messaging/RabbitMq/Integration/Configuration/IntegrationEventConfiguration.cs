using Fusion.Messaging.RabbitMq.Abstractions;
using Fusion.Messaging.RabbitMq.Events;
using Fusion.Messaging.RabbitMq.Handlers;
using Fusion.Messaging.RabbitMq.Helpers;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using ConfigurationExtensions = Fusion.Microservice.Extensions.ConfigurationExtensions;
using IBusFactoryConfigurator = Fusion.Messaging.RabbitMq.Abstractions.IBusFactoryConfigurator;

namespace Fusion.Messaging.RabbitMq.Integration.Configuration;

/// <summary>
/// Provides methods to configure messaging components using MassTransit
/// for external integration message handling and consumer registration.
/// </summary>
public static class IntegrationEventConfiguration
{
    /// <summary>
    /// Adds a custom event consumer to the message bus, configuring its associated endpoint.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event being handled. Must implement <see cref="IEvent"/>.</typeparam>
    /// <typeparam name="TConsumer">The type of the consumer processing the event. Must implement <see cref="IEventConsumer{TEvent}"/>.</typeparam>
    /// <param name="configurator">The bus factory configurator used to configure the message bus.</param>
    /// <param name="context">The messaging registration context for resolving dependencies required by the consumer.</param>
    /// <param name="queueName">The name of the queue to which the endpoint will be bound.</param>
    public static void AddCustomEvent<TEvent, TConsumer>(this IBusFactoryConfigurator configurator,
        IMessagingRegistrationContext context, string queueName) where TConsumer : class, IEventConsumer<TEvent>
        where TEvent : class, IEvent
    {
        configurator.ConfigureMessage<TEvent>(queueName);
        configurator.ReceiveEndpoint(queueName, e =>
        {
            e.SetDurable(true);
            e.ConfigureConsumer<TConsumer>(context);
        });
    }

    /// <summary>
    /// Configures an inbound consumer for the message bus.
    /// </summary>
    /// <param name="configurator">The bus factory configurator used to configure the message bus.</param>
    /// <param name="context">The messaging registration context for resolving dependencies.</param>
    /// <param name="configuration">The application configuration used to access service-specific settings.</param>
    /// <param name="externalServices">A list of external service exchange names to bind to the queue.</param>
    public static void ConfigureInboundConsumer(
        this IBusFactoryConfigurator configurator,
        IMessagingRegistrationContext context,
        IConfiguration configuration,
        List<string> externalServices
    )
    {
        var serviceName = ConfigurationExtensions.GetServiceOptions(configuration).ServiceName;
        var queueName = $"{EventNameHelpers.NormalizeName(serviceName)}-inbound";
        configurator.ReceiveEndpoint(queueName, e =>
        {
            e.SetDurable(true);
            foreach (var serviceExchange in externalServices)
            {
                e.Bind(serviceExchange, b => { b.SetExchangeType(ExchangeType.Direct); });
            }

            e.ConfigureConsumer<EventConsumer>(context);
        });
    }
}