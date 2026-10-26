namespace Fusion.Messaging.RabbitMq.Abstractions;

/// <summary>
/// Provides functionality to configure a message bus, including host settings,
/// default exchanges, filters for message consumption, and receive endpoints.
/// </summary>
public interface IBusFactoryConfigurator
{
    /// <summary>
    /// Configures the host with the specified connection details.
    /// </summary>
    /// <param name="host">
    /// The host address to connect to.
    /// </param>
    /// <param name="username">
    /// The username for authentication.
    /// </param>
    /// <param name="password">
    /// The password for authentication.
    /// </param>
    /// <param name="timeoutBySeconds">The timeout by seconds for broker.</param>
    /// <param name="heartbeatBySeconds">The heartbeat by seconds for broker.</param>
    void ConfigureHost(
        string host,
        string username,
        string password,
        int timeoutBySeconds = 60,
        int heartbeatBySeconds = 10
    );

    /// <summary>
    /// Configures retry behavior with specified limits and time intervals.
    /// </summary>
    /// <param name="limit">The maximum number of retry attempts.</param>
    /// <param name="minIntervalBySeconds">The minimum interval, in seconds, between retries.</param>
    /// <param name="maxIntervalBySeconds">The maximum interval, in seconds, between retries.</param>
    /// <param name="intervalDeltaBySeconds">The incremental interval increase, in seconds, applied after each retry attempt.</param>
    public void ConfigureRetry(
        int limit = 5,
        int minIntervalBySeconds = 1,
        int maxIntervalBySeconds = 30,
        int intervalDeltaBySeconds = 3
    );

    /// <summary>
    /// Configures the default exchange for the specified service.
    /// </summary>
    /// <param name="serviceName">
    /// The name of the service for which the default exchange is to be configured.
    /// </param>
    void ConfigureDefaultExchange(string serviceName);

    /// <summary>
    /// Configures message handling for a specific event type and binds it to the given queue.
    /// </summary>
    /// <param name="queueName">The name of the queue to which the event messages should be routed.</param>
    /// <typeparam name="TEvent">The type of the event message being configured.</typeparam>
    void ConfigureMessage<TEvent>(string queueName) where TEvent : class;

    /// <summary>
    /// Configures a receive endpoint for a specified queue.
    /// </summary>
    /// <param name="queueName">
    /// The name of the queue to bind to the receive endpoint.
    /// </param>
    /// <param name="configure">
    /// An action to configure the receive endpoint using the provided <see cref="IReceiveEndpointConfigurator"/>.
    /// </param>
    void ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configure);
}