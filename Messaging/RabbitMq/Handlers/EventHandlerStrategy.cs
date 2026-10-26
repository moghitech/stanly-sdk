using Fusion.Messaging.RabbitMq.Registration.Configurators;

namespace Fusion.Messaging.RabbitMq.Handlers;

/// <inheritdoc cref="IEventHandlerStrategy"/>
public sealed class EventHandlerStrategy(
    IServiceProvider serviceProvider,
    IntegrationSetupConfigurator integrationSetupConfigurator
) : IEventHandlerStrategy
{
    /// <inheritdoc cref="IEventHandlerStrategy.GetHandler"/>
    public IEventHandler? GetHandler(string eventType)
    {
        integrationSetupConfigurator.TryGetHandler(eventType, out var factory);

        return factory?.Invoke(serviceProvider);
    }
}