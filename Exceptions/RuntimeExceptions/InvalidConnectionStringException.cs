namespace Fusion.Exceptions.RuntimeExceptions;

/// <summary>
/// Represents an exception that is thrown when an invalid connection string is provided.
/// </summary>
public class InvalidConnectionStringException(string message) : RuntimeException(message);