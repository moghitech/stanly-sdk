using System.Text.Json;
using Fusion.Exceptions;
using Fusion.Exceptions.BusinessExceptions;
using Fusion.Exceptions.ServiceExceptions;
using Fusion.Microservice.Domain;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Fusion.Microservice.Middleware;

/// <summary>
/// Middleware that handles exceptions globally for the microservice.
/// It logs the exception, determines the appropriate HTTP status code,
/// and returns a structured JSON response to the client.
/// </summary>
/// <param name="logger">Logger instance used to log the exception details.</param>
public class ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) : IExceptionHandler
{
    /// <summary>
    /// Attempts to handle an exception and generate an appropriate response.
    /// It logs the exception and sets the correct HTTP status code and message.
    /// </summary>
    /// <param name="httpContext">The HTTP context for the request.</param>
    /// <param name="exception">The exception that occurred.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, with a boolean indicating whether the exception was handled.</returns>
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        // Log the exception details
        logger.LogError($"{nameof(ExceptionHandlingMiddleware)}: {{exception}}", exception.Message);

        // Set the response content type to JSON
        httpContext.Response.ContentType = "application/json";

        ApiResult? response;
        if (exception is BusinessException businessException)
        {
            // Handle business exceptions with a custom localized message or default message
            var message = businessException.LocalizedMessage() ?? "درخواست شما با شکست مواجه شد.";
            response = ApiResult.FromServerError(message);
        }
        else
        {
            // Handle all other exceptions with a generic message
            response = ApiResult.FromServerError(exception.Message);
        }

        // Set the appropriate HTTP status code for the exception type
        httpContext.Response.StatusCode = GetExceptionStatusCode(exception);

        // Write the response to the client
        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response), cancellationToken);

        return true; // Indicate the exception was handled
    }

    /// <summary>
    /// Maps the exception type to the appropriate HTTP status code.
    /// </summary>
    /// <param name="exception">The exception to map.</param>
    /// <returns>The HTTP status code corresponding to the exception type.</returns>
    private static int GetExceptionStatusCode(Exception exception)
    {
        return exception switch
        {
            BusinessException ex => ex.Status ?? StatusCodes.Status400BadRequest,
            ValidationException or ArgumentOutOfRangeException => StatusCodes.Status400BadRequest,
            NotFoundException => StatusCodes.Status404NotFound,
            UnauthorizedException => StatusCodes.Status401Unauthorized,
            InternalServiceNotAvailableException => StatusCodes.Status503ServiceUnavailable,
            _ => StatusCodes.Status500InternalServerError // Default to 500 for unhandled exceptions
        };
    }
}