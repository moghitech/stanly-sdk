namespace Fusion.Exceptions.ServiceExceptions;

/// <summary>
/// Represents an exception that occurs when a service call fails.
/// This exception contains details about the service name, method, and error message.
/// </summary>
/// <param name="serviceName">The name of the service where the error occurred.</param>
/// <param name="serviceMethod">The method of the service that failed.</param>
/// <param name="errorMessage">The error message returned from the service.</param>
public class InternalServiceNotAvailableException(
    string serviceName,
    string serviceMethod,
    string errorMessage
) : RuntimeException(
    $"Service call failed, [ServiceName]=({serviceName}), [ServiceMethod]=({serviceMethod}), [ErrorMessage]=({errorMessage})");