using Fusion.Messaging.RabbitMq.Abstractions;
using MassTransit;

namespace Fusion.Messaging.RabbitMq.Internals.MassTransit;

/// <inheritdoc cref="Abstractions.IReceiveEndpointConfigurator"/>
internal class MassTransitReceiveEndpointConfigurator(IRabbitMqReceiveEndpointConfigurator inner)
    : Abstractions.IReceiveEndpointConfigurator
{
    /// <inheritdoc cref="Abstractions.IReceiveEndpointConfigurator.SetDurable"/>
    public void SetDurable(bool durable)
    {
        inner.Durable = durable;
    }

    /// <inheritdoc cref="Abstractions.IReceiveEndpointConfigurator.Bind"/>
    public void Bind(string exchange, Action<IBindConfigurator> configure)
    {
        inner.Bind(exchange, b => configure(new MassTransitBindConfigurator(b)));
    }

    /// <inheritdoc cref="Abstractions.IReceiveEndpointConfigurator.ConfigureConsumer{TConsumer}"/>
    public void ConfigureConsumer<TConsumer>(IMessagingRegistrationContext context)
        where TConsumer : class, IConsumer
    {
        var massTransitContext = (MassTransitRegistrationContext)context;
        inner.ConfigureConsumer<TConsumer>(massTransitContext.Inner);
    }
}