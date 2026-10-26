using Fusion.Messaging.RabbitMq.Abstractions;
using MassTransit;

namespace Fusion.Messaging.RabbitMq.Internals.MassTransit;

/// <inheritdoc cref="IMessagingRegistrationConfigurator"/>
internal class MassTransitRegistrationConfigurator(IBusRegistrationConfigurator inner)
    : IMessagingRegistrationConfigurator
{
    /// <inheritdoc cref="IMessagingRegistrationConfigurator.AddConsumer{TConsumer}"/>
    public void AddConsumer<TConsumer>() where TConsumer : class, IConsumer
    {
        inner.AddConsumer<TConsumer>();
    }

    /// <inheritdoc cref="IMessagingRegistrationConfigurator.AddConsumer"/>
    public void AddConsumer(Type consumerType)
    {
        inner.AddConsumer(consumerType);
    }
}