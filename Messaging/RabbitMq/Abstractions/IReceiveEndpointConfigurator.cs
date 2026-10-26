using MassTransit;

namespace Fusion.Messaging.RabbitMq.Abstractions;

/// <summary>
/// Provides the configuration options for setting up a receive endpoint.
/// The receive endpoint serves as a consumer endpoint for receiving messages from a specific queue or exchange.
/// </summary>
public interface IReceiveEndpointConfigurator
{
    /// <summary>
    /// Configures whether the receive endpoint should be durable.
    /// When set to true, the endpoint will persist its configuration and state
    /// even in case of a broker restart.
    /// </summary>
    /// <param name="durable">A boolean indicating whether the endpoint should be durable.</param>
    void SetDurable(bool durable);

    /// <summary>
    /// Binds a specified exchange to the receive endpoint, allowing configuration of binding options
    /// through the provided binding configurator.
    /// </summary>
    /// <param name="exchange">The name of the exchange to bind.</param>
    /// <param name="configure">An action to configure the binding options for the exchange.</param>
    void Bind(string exchange, Action<IBindConfigurator> configure);

    /// <summary>
    /// Configures a consumer for the receive endpoint using the provided messaging registration context.
    /// This method allows integration of a specific consumer type with the messaging infrastructure
    /// in the context of a receive endpoint.
    /// </summary>
    /// <typeparam name="TConsumer">The type of the consumer to be configured.</typeparam>
    /// <param name="context">The messaging registration context used for configuring the consumer.</param>
    void ConfigureConsumer<TConsumer>(IMessagingRegistrationContext context) where TConsumer : class, IConsumer;
}