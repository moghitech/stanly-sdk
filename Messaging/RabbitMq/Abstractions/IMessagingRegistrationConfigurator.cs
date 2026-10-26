using MassTransit;

namespace Fusion.Messaging.RabbitMq.Abstractions;

/// <summary>
/// Provides configuration methods for registering messaging consumers within the messaging system.
/// </summary>
public interface IMessagingRegistrationConfigurator
{
    /// <summary>
    /// Registers a messaging consumer of the specified type into the messaging system configuration.
    /// </summary>
    /// <typeparam name="TConsumer">The type of the consumer to be added. This type must implement the <see cref="IConsumer"/> interface.</typeparam>
    void AddConsumer<TConsumer>() where TConsumer : class, IConsumer;

    /// <summary>
    /// Registers a messaging consumer of the specified type into the messaging system configuration.
    /// </summary>
    /// <param name="consumerType">The type of the consumer to be added. This type must implement the <see cref="IConsumer"/> interface.</param>
    void AddConsumer(Type consumerType);
}