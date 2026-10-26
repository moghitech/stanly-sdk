namespace Fusion.Messaging.RabbitMq.Abstractions;

/// <summary>
/// Provides a contract for configuring binding options in a messaging system.
/// </summary>
public interface IBindConfigurator
{
    /// <summary>
    /// Sets the type of the exchange for message binding configurations.
    /// </summary>
    /// <param name="type">The exchange type to be set. Common values are "direct", "fanout", "topic", or "headers".</param>
    void SetExchangeType(string type);
}