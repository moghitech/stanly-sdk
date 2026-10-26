using System.Net;

namespace Fusion.Exceptions.BusinessExceptions;

/// <summary>
/// Represents a business-specific exception that can be used to handle errors
/// occurring due to violations of business logic or rules within an application.
/// </summary>
/// <remarks>
/// The <see cref="BusinessException"/> class provides support for storing localized
/// error messages and an HTTP status code for finer-grained error handling.
/// This exception is commonly used to denote client-side issues or business rule violations
/// such as validation errors, bad requests, or other scenarios where special
/// handling is required in the application's middleware.
/// </remarks>
public class BusinessException : Exception
{
    private readonly string? _localizedMessage;

    /// <summary>
    /// Gets the HTTP status code associated with the business-specific exception.
    /// </summary>
    /// <remarks>
    /// The <c>Status</c> property provides an integer representation of an HTTP status code
    /// that corresponds to the specific business rule violation or error scenario.
    /// This can be used to facilitate appropriate response handling and communication with clients.
    /// </remarks>
    public int? Status { get; private set; }

    /// <summary>
    /// Represents a business-specific exception that handles scenarios where violations of business logic or rules occur.
    /// </summary>
    /// <remarks>
    /// This exception provides the capability to include localized error messages and custom HTTP status codes for enhanced error handling.
    /// Use the <see cref="BusinessException"/> class to encapsulate detailed information about domain-specific validation errors, bad requests, or
    /// business rule infringements requiring specialized treatment in middleware or other application layers.
    /// </remarks>
    public BusinessException(string message) : base(message)
    {
        Status = null;
    }

    /// <summary>
    /// Represents a business-specific exception used to manage errors resulting from violations of business rules or logic.
    /// </summary>
    /// <remarks>
    /// The <see cref="BusinessException"/> class supports features such as localized error messages and HTTP status codes to enable detailed error management.
    /// It is commonly applied in scenarios like domain rule validations, bad requests, or any business rule infringements that require specific handling in middleware or client systems.
    /// </remarks>
    public BusinessException(string? message, string? localizedMessage) 
        : base(message)
    {
        Status = null;
        _localizedMessage = localizedMessage;
    }

    /// <summary>
    /// Represents a business-specific exception used to handle errors caused by violations of business rules or logic.
    /// </summary>
    /// <remarks>
    /// The <see cref="BusinessException"/> class is designed to facilitate granular error handling by supporting localized error messages and HTTP status codes. It is commonly utilized for application scenarios such as domain validations, bad requests, or custom business rule failures, enhancing middleware-level exception handling.
    /// </remarks>
    public BusinessException(List<string> messages, List<string> localizedMessages) 
        : base(string.Join("\n", messages))
    {
        Status = null;
        _localizedMessage = string.Join("\n", localizedMessages);
    }

    /// <summary>
    /// Represents an exception type for handling violations of business logic or rules within an application.
    /// </summary>
    /// <remarks>
    /// The <see cref="BusinessException"/> class supports providing localized error messages,
    /// HTTP status codes, and structured error messages to facilitate precise error handling
    /// in scenarios such as domain validation failures, client-side constraints, or bad requests.
    /// </remarks>
    public BusinessException(string? message, string? localizedMessage, Exception? innerException)
        : base(message, innerException)
    {
        Status = null;
        _localizedMessage = localizedMessage;
    }

    /// <summary>
    /// Represents an exception type used to handle business logic or rule violations within an application.
    /// </summary>
    /// <remarks>
    /// The <see cref="BusinessException"/> class is designed to encapsulate errors that occur due to
    /// business logic constraints, client-side issues, or domain-specific validation failures. It supports
    /// features for localized error messaging and HTTP status codes, enabling more precise error management.
    /// </remarks>
    public BusinessException(string message, string? localizedMessage, HttpStatusCode status) : base(message)
    {
        _localizedMessage = localizedMessage;
        Status = (int)status;
    }

    /// <summary>
    /// Retrieves the localized message associated with this exception.
    /// </summary>
    /// <returns>
    /// A string containing the localized message if available; otherwise, null.
    /// This message is intended to provide user-friendly, localized feedback regarding the exception context.
    /// </returns>
    public string? LocalizedMessage()
    {
        return _localizedMessage;
    }
}