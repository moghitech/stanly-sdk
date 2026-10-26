using Fusion.Helpers;
using System.Net;
using System.Net.Http.Json;
using System.Net.Mime;

namespace Fusion.Microservice.Domain;

/// <summary>
/// Represents the result of an HTTP operation, encapsulating information about success status, HTTP status code,
/// and an optional message, with optional strongly typed data for the operation result when generic.
/// </summary>
public record HttpResult<TData>(
    bool IsSuccess,
    HttpStatusCode? StatusCode,
    string Message
) : HttpResult(IsSuccess, StatusCode, Message)
    where TData : class
{
    /// <summary>
    /// Represents the data associated with the HTTP result.
    /// </summary>
    /// <remarks>
    /// This property holds the response payload of type <c>TData</c> when the HTTP request is successful.
    /// The value will be <c>null</c> if the HTTP request fails or does not include data.
    /// </remarks>
    public TData? Data { get; set; }

    /// <summary>
    /// Creates a new <see cref="HttpResult{TData}"/> instance by processing the provided HTTP response message.
    /// </summary>
    /// <param name="response">The HTTP response message containing the status code, headers, and content to process.</param>
    /// <returns>An instance of <see cref="HttpResult{TData}"/> populated with the HTTP response details and deserialized data, if applicable.</returns>
    public new static async Task<HttpResult<TData>> FromHttpResponseAsync(HttpResponseMessage response)
    {
        var message = response.IsSuccessStatusCode
            ? "Success"
            : await response.Content.ReadAsStringAsync();

        var data = default(TData);

        if (response.IsSuccessStatusCode)
        {
            var contentType = response.Content.Headers.ContentType?.MediaType;
            if (contentType == MediaTypeNames.Application.Json)
            {
                data = await response.Content.ReadFromJsonAsync<TData>();
            }
            else if (contentType is MediaTypeNames.Application.Xml or MediaTypeNames.Text.Xml)
            {
                data = XmlHelper.FromXml<TData>(await response.Content.ReadAsStringAsync());
            }
        }

        return new HttpResult<TData>(response.IsSuccessStatusCode,
            response.StatusCode,
            message)
        {
            Data = data
        };
    }

    /// <summary>
    /// Creates a new <see cref="HttpResult{TData}"/> instance by processing the provided exception.
    /// </summary>
    /// <param name="exception">The exception containing details about the error or failure to process.</param>
    /// <returns>An instance of <see cref="HttpResult{TData}"/> populated with error details derived from the supplied exception.</returns>
    public new static HttpResult<TData> FromException(Exception exception)
    {
        return new HttpResult<TData>(false,
            exception is HttpRequestException httpRequestException
                ? httpRequestException.StatusCode
                : null,
            exception.Message
        );
    }
}

/// <summary>
/// Represents the result of an HTTP operation, providing details about whether the operation was successful,
/// the corresponding HTTP status code, and an optional message describing the result.
/// </summary>
public record HttpResult(
    bool IsSuccess,
    HttpStatusCode? StatusCode,
    string Message
)
{
    /// <summary>
    /// Indicates whether the HTTP result represents a failure.
    /// </summary>
    /// <remarks>
    /// The property is derived from the inverse of the <c>IsSuccess</c> property.
    /// It evaluates to <c>true</c> when the HTTP result is not successful, otherwise <c>false</c>.
    /// </remarks>
    public bool IsFailed => !IsSuccess;

    /// <summary>
    /// Creates a new <see cref="HttpResult{TData}"/> instance by processing the provided HTTP response.
    /// </summary>
    /// <param name="response">The HTTP response message to process, containing status, headers, and content information.</param>
    /// <returns>An instance of <see cref="HttpResult{TData}"/> populated with the HTTP response details and deserialized data if applicable.</returns>
    public static async Task<HttpResult> FromHttpResponseAsync(HttpResponseMessage response)
    {
        var message = await response.Content.ReadAsStringAsync();
        return new HttpResult(response.IsSuccessStatusCode,
            response.StatusCode,
            message
        );
    }

    /// <summary>
    /// Creates a new <see cref="HttpResult{TData}"/> instance to represent an error state based on the provided exception.
    /// </summary>
    /// <param name="exception">The exception used to populate the error details, including its message and potential status code.</param>
    /// <returns>A failure instance of <see cref="HttpResult{TData}"/> with the exception details.</returns>
    public static HttpResult FromException(Exception exception)
    {
        return new HttpResult(false,
            HttpStatusCode.InternalServerError,
            exception.Message
        );
    }
}