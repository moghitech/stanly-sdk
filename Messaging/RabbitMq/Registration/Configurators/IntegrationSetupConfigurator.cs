using Fusion.Messaging.RabbitMq.Handlers;
using Fusion.Messaging.RabbitMq.Integration.Abstractions;
using Fusion.Messaging.RabbitMq.Integration.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fusion.Messaging.RabbitMq.Registration.Configurators;

/// <summary>
/// Provides configuration settings for integration setup in the messaging system.
/// Allows defining a default consumer type and configuring external services for integration events.
/// </summary>
public class IntegrationSetupConfigurator
{
    /// <summary>
    /// A dictionary that maps event type names to factory methods for creating event handlers.
    /// </summary>
    private readonly Dictionary<string, Func<IServiceProvider, IEventHandler>> _handlerFactories = new();

    /// <summary>
    /// A dictionary that maps event type names to their corresponding handler types.
    /// </summary>
    private readonly Dictionary<string, Type> _handlerTypes = new();

    /// <summary>
    /// Defines the integration configuration and setup for external systems or services.
    /// </summary>
    /// <remarks>
    /// This property encapsulates the details required for establishing and managing
    /// the connection and interaction with external resources or processes. It is a key element
    /// in enabling seamless communication and data exchange between systems in the environment.
    /// </remarks>
    public IntegrationConfigurator? Integration { get; } = new();

    /// <summary>
    /// Specifies the collection of external services that the integration setup relies on.
    /// </summary>
    /// <remarks>
    /// External services are typically identified by their unique names or identifiers.
    /// This property serves to track these services, allowing for configuration or registration
    /// of necessary components or consumers that interact with these services in the messaging system.
    /// </remarks>
    public List<string> ExternalServices { get; } = [];

    /// <summary>
    /// Sets the external services by adding the provided collection of service identifiers to the existing list of external services.
    /// </summary>
    /// <param name="externalServices">
    /// A collection of external service identifiers to be added to the existing list.
    /// </param>
    public void SetExternalServices(IEnumerable<string> externalServices)
    {
        ExternalServices.AddRange(externalServices);
    }

    /// <summary>
    /// Registers an event type and its associated handler in the handler factories.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event to be handled.</typeparam>
    /// <typeparam name="THandler">The implementation of the event handler for the specified event type.</typeparam>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the handler cannot be resolved or when the event type name cannot be determined.
    /// </exception>
    public void AddHandler<TEvent, THandler>() where TEvent : IEntityEvent where THandler : class, IEventHandler<TEvent>
    {
        var eventTypeName = typeof(TEvent).AssemblyQualifiedName ?? throw new InvalidOperationException();

        _handlerTypes.Add(eventTypeName, typeof(THandler));

        _handlerFactories[eventTypeName] = sp =>
        {
            var handler = sp.GetService<THandler>() ??
                          throw new InvalidOperationException($"Failed to resolve {typeof(THandler).FullName}");
            return new EventHandlerWrapper<TEvent>(handler);
        };
    }

    /// <summary>
    /// Attempts to retrieve the factory method for creating an event handler based on the specified event type.
    /// </summary>
    /// <param name="eventType">The type name of the event for which the handler factory is requested.</param>
    /// <param name="factory">The factory method for creating the event handler, if found; otherwise, <c>null</c>.</param>
    public void TryGetHandler(string eventType, out Func<IServiceProvider, IEventHandler>? factory)
    {
        factory = _handlerFactories.TryGetValue(eventType, out var handlerDelegate) ? handlerDelegate : null;
    }

    /// <summary>
    /// Returns a dictionary that maps event type names to their corresponding handler types.
    /// </summary>
    /// <returns>
    /// A <see cref="Dictionary{TKey, TValue}"/> where the key is a string representing the event type name,
    /// and the value is a <see cref="Type"/> representing the corresponding handler type.
    /// </returns>
    public Dictionary<string, Type> GetHandlerTypes()
    {
        return _handlerTypes;
    }
}