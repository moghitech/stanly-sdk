using Fusion.Messaging.RabbitMq.Abstractions;
using MassTransit;

namespace Fusion.Messaging.RabbitMq.Internals.MassTransit;

/// <inheritdoc cref="IBindConfigurator"/>
internal class MassTransitBindConfigurator(IRabbitMqExchangeToExchangeBindingConfigurator inner) : IBindConfigurator
{
    /// <inheritdoc cref="IBindConfigurator.SetExchangeType"/>
    public void SetExchangeType(string type)
    {
        inner.ExchangeType = type;
    }
}