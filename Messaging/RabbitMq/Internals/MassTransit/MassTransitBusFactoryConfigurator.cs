using Fusion.Messaging.RabbitMq.Helpers;
using Fusion.Messaging.RabbitMq.Integration.Abstractions;
using MassTransit;
using IBusFactoryConfigurator = Fusion.Messaging.RabbitMq.Abstractions.IBusFactoryConfigurator;
using IReceiveEndpointConfigurator = Fusion.Messaging.RabbitMq.Abstractions.IReceiveEndpointConfigurator;

namespace Fusion.Messaging.RabbitMq.Internals.MassTransit;

/// <inheritdoc cref="IBusFactoryConfigurator{TEndpointConfigurator}"/>
internal class MassTransitBusFactoryConfigurator(IRabbitMqBusFactoryConfigurator inner)
    : IBusFactoryConfigurator
{
    /// <inheritdoc cref="Abstractions.IBusFactoryConfigurator.ConfigureHost"/>
    public void ConfigureHost(
        string host,
        string username,
        string password,
        int timeoutBySeconds = 60,
        int heartbeatBySeconds = 10
    )
    {
        inner.Host(host, h =>
        {
            h.Username(username);
            h.Password(password);
            h.Heartbeat(TimeSpan.FromSeconds(heartbeatBySeconds));
            h.RequestedConnectionTimeout(TimeSpan.FromSeconds(timeoutBySeconds));
        });
    }

    /// <inheritdoc cref="Abstractions.IBusFactoryConfigurator.ConfigureRetry"/>
    public void ConfigureRetry(
        int limit = 5,
        int minIntervalBySeconds = 1,
        int maxIntervalBySeconds = 30,
        int intervalDeltaBySeconds = 3
    )
    {
        inner.UseMessageRetry(rc => rc.Exponential(
            retryLimit: limit,
            minInterval: TimeSpan.FromSeconds(minIntervalBySeconds),
            maxInterval: TimeSpan.FromSeconds(maxIntervalBySeconds),
            intervalDelta: TimeSpan.FromSeconds(intervalDeltaBySeconds)
        ));

        inner.UseCircuitBreaker(cb => cb.TrackingPeriod = TimeSpan.FromMinutes(1));
    }

    /// <inheritdoc cref="Abstractions.IBusFactoryConfigurator.ConfigureDefaultExchange"/>
    public void ConfigureDefaultExchange(string serviceName)
    {
        inner.Message<BaseEntityEvent>(m =>
        {
            m.SetEntityName($"{EventNameHelpers.NormalizeName(serviceName)}-exchange");
        });
        inner.Publish<BaseEntityEvent>(e =>
        {
            e.ExchangeType = "direct";
            e.Exclude = true;
            e.Durable = true;
            e.AutoDelete = false;
        });
    }

    /// <inheritdoc cref="Abstractions.IBusFactoryConfigurator.ConfigureMessage{TEvent}"/>
    public void ConfigureMessage<TEvent>(string queueName) where TEvent : class
    {
        inner.Message<TEvent>(m => { m.SetEntityName(queueName); });
    }

    /// <inheritdoc cref="Abstractions.IBusFactoryConfigurator.ReceiveEndpoint"/>
    public void ReceiveEndpoint(string queueName,
        Action<IReceiveEndpointConfigurator> configure)
    {
        inner.ReceiveEndpoint(queueName, e => configure(new MassTransitReceiveEndpointConfigurator(e)));
    }
}