namespace Fusion.Microservice.Extensions.Options;

/// <summary>
/// Provides configuration options for connecting to a RabbitMQ server.
/// Contains properties for specifying the server host, authentication credentials, and port.
/// </summary>
public class RabbitMqOptions
{
    /// <summary>
    /// Gets or sets the host address of the RabbitMQ server.
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    /// Gets or sets the username used for authentication with the RabbitMQ server.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Gets or sets the password used for authentication with the RabbitMQ server.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Gets or sets the port number used to connect to the RabbitMQ server.
    /// </summary>
    public int Port { get; set; }
}