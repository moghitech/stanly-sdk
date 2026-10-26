namespace Fusion.Exceptions;

/// <summary>
/// Represents a runtime exception, which serves as a base class for specific exceptions
/// occurring during the execution of a program.
/// This exception provides a mechanism to handle errors that are typically not recoverable
/// and indicate a problem in the program logic or configuration.
/// </summary>
public class RuntimeException(string message) : Exception(message: message);