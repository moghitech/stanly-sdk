namespace Fusion.Exceptions.RuntimeExceptions;

/// <summary>
/// Represents an exception that is thrown when a required connection string is not properly registered
/// or is missing in the application configuration settings.
/// </summary>
public class ConnectionStringException(string connectionStringName)
    : RuntimeException($"Please check the appSettings. The connectionString is not registered. ConnectionStringName: {connectionStringName}");