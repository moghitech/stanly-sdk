using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fusion.Microservice.Domain;

/// <summary>
/// The base structure of api responses those are Ok state
/// </summary>
public class ApiResult : IActionResult
{
    /// <summary>
    /// Indicates the HTTP status code of the current API response.
    /// </summary>
    protected int StatusCode { get; init; } = StatusCodes.Status200OK;

    /// <summary>
    /// Represents the base structure for uniform API responses.
    /// </summary>
    public ApiResult(string? message = null)
    {
        IsSuccess = true;
        Message = message;
    }

    /// <summary>
    /// A custom message that return to client
    /// </summary>
    public string? Message { get; init; }

    /// <summary>
    /// Represents detailed validation errors associated with the request,
    /// where the key is the name of the invalid field, and the value is a list of error messages for that field.
    /// </summary>
    public Dictionary<string, List<string>> ValidationErrors { get; set; } = [];

    /// <summary>
    /// Indicates whether the operation represented by the API response was successful or not.
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// By calling the function,ApiResult behaves like NoContent result (204 HttpStatusCode)
    /// </summary>
    public static ApiResult NoContent()
    {
        return new ApiResult
        {
            StatusCode = StatusCodes.Status204NoContent,
            IsSuccess = true
        };
    }

    /// <summary>
    /// By calling the function,ApiResult behaves like NoContent result (204 HttpStatusCode)
    /// </summary>
    public static ApiResult FromServerError(string message = "Internal Server Error")
    {
        return new ApiResult
        {
            StatusCode = StatusCodes.Status500InternalServerError,
            IsSuccess = false,
            Message = message
        };
    }

    /// <summary>
    /// Executes the result operation asynchronously.
    /// Determines the result type based on the current configuration and writes it to the response.
    /// </summary>
    /// <param name="context">The context in which the result is executed, including the HTTP context and route data.</param>
    /// <returns>A task that represents the asynchronous operation of executing the result.</returns>
    Task IActionResult.ExecuteResultAsync(ActionContext context)
    {
        if (StatusCode == StatusCodes.Status204NoContent)
        {
            return new NoContentResult().ExecuteResultAsync(context);
        }

        return new ObjectResult(this)
        {
            StatusCode = this.StatusCode
        }.ExecuteResultAsync(context);
    }

    /// <summary>
    /// By calling this function, ApiResult behaves like bad request from client(400)
    /// </summary>
    /// <param name="message">The validation message should be return as bad request.</param>
    /// <returns></returns>
    public static ApiResult BadRequest(string? message = "Bad Request!")
    {
        return new ApiResult
        {
            IsSuccess = false,
            Message = message,
            StatusCode = StatusCodes.Status400BadRequest,
        };
    }

    /// <summary>
    /// Creates an ApiResult instance representing an entity creation response with HTTP 201 status code.
    /// </summary>
    /// <param name="message">The message associated with the created response.</param>
    /// <returns>An ApiResult instance indicating success and containing the provided message.</returns>
    public static ApiResult Created(string? message = "Entity created!")
    {
        return new ApiResult
        {
            IsSuccess = true,
            Message = message,
            StatusCode = StatusCodes.Status201Created,
        };
    }

    /// <summary>
    /// Specifies a method or function that defines a custom behavior for a specific operator in a class or struct.
    /// </summary>
    public static implicit operator ApiResult(BadRequestObjectResult badRequest)
    {
        var message = badRequest.Value?.ToString();

        return new ApiResult()
        {
            IsSuccess = false,
            Message = message,
            StatusCode = StatusCodes.Status400BadRequest,
        };
    }
}

/// <summary>
/// Represents the base class for API responses, providing a standard structure to interact with.
/// </summary>
public class ApiResult<TData> : ApiResult
{
    /// <summary>
    /// Represents the result of an API operation, encapsulating status, message, data, and validation errors.
    /// </summary>
    public ApiResult(TData? data)
    {
        Data = data;
        IsSuccess = true;
    }

    /// <summary>
    /// A Generic type that contains the response body
    /// </summary>
    /// <value></value>
    public TData? Data { get; }

    /// <summary>
    /// By calling this function, ApiResult behaves like created from client(201)
    /// </summary>
    /// <param name="data">The data should return in ApiResult.</param>
    /// <param name="message">The validation message should be return as bad request.</param>
    /// <returns></returns>
    public static ApiResult<TData> Created(TData data, string? message = "Entity created!")
    {
        return new ApiResult<TData>(data);
    }


    /// <summary>
    /// Represents a bad request response with an optional message.
    /// </summary>
    /// <param name="message">The message describing the bad request.</param>
    /// <returns>An instance of ApiResult representing the bad request.</returns>
    public new static ApiResult<TData> BadRequest(string? message = "Bad Request!")
    {
        return new ApiResult<TData>(default)
        {
            IsSuccess = false,
            Message = message
        };
    }

    /// <summary>
    /// Represents a bad request response.
    /// </summary>
    /// <returns>An instance of ApiResult representing the bad request.</returns>
    public static ApiResult<TData> BadRequest(params KeyValuePair<string, List<string>>[] validationMessages)
    {
        return new ApiResult<TData>(default)
        {
            IsSuccess = false,
            Message = "Bad Request!",
            ValidationErrors = validationMessages.ToDictionary(s => s.Key, s => s.Value),
        };
    }

    /// <summary>
    /// Defines a custom implementation or overload for a specific operator in a type.
    /// </summary>
    public static implicit operator ApiResult<TData>(TData value) => new(value);
}