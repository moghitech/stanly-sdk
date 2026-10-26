using System.Net;
using FluentValidation.Results;

namespace Fusion.Exceptions.BusinessExceptions;

/// <summary>
/// Represents an exception that occurs during validation failures, inheriting from the BaseHttpException.
/// </summary>
/// <remarks>
/// The ValidationException is specifically used to encapsulate validation errors
/// in the context of HTTP-based exceptions. This exception is typically thrown when
/// there are validation errors in user input or business logic, providing relevant
/// details about the failures through a structured format.
/// </remarks>
/// <example>
/// This exception collects multiple validation failures and organizes them into
/// a dictionary, where the key represents the property name and the value is a list
/// of validation error messages related to that property.
/// </example>
/// <exception cref="BaseHttpException">
/// This exception inherits from BaseHttpException to provide HTTP-specific attributes
/// for error handling, such as status codes and bulk error messages.
/// </exception>
/// <seealso cref="BaseHttpException"/>
public class ValidationException(List<ValidationFailure> messages) : BaseHttpException(message: "Unprocessable Entity",
    status: HttpStatusCode.UnprocessableEntity,
    bulkMessages: InitContext(messages: messages))
{
    private static Dictionary<string, List<string>> InitContext(List<ValidationFailure> messages)
    {
        Dictionary<string, List<string>> initMessages = new();

        foreach (var error in messages)
        {
            if (!initMessages.TryGetValue(error.PropertyName, out var value))
            {
                value = [];
                initMessages[error.PropertyName] = value;
            }

            value.Add(error.ErrorMessage);
        }

        return initMessages;
    }
}