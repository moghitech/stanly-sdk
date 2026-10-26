using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Fusion.Microservice.Domain;
using HttpClientToCurl;
using Newtonsoft.Json;
using Serilog;

namespace Fusion.Extensions;

/// <summary>
/// A builder class for constructing and executing HTTP requests with various configurations.
/// </summary>
public class HttpRequestBuilder(HttpClient httpClient)
{
    private Uri BaseUrl { get; set; } = null!;
    private string Url { get; set; } = null!;

    private HttpMethod HttpMethod { get; set; } = HttpMethod.Get;

    private object? JsonBody { get; set; }
    private bool AcceptOnlyJson { get; set; }

    private bool _ensureSuccessStatusCode;

    private bool _generateCurlScript;
    private ILogger _logger = null!;
    private string _logKey = "HttpRequestBuilder";

    private readonly Dictionary<string, string> _formParameters = new();
    private MultipartFormDataContent? _multipartContent;

    private readonly Dictionary<HttpStatusCode, Func<Task>> _onFailedApiResultFuncDictionary = new();

    private int? TimeOutInSecond { get; set; }

    private readonly IDictionary<string, string> _queryParams = new Dictionary<string, string>();
    private readonly IDictionary<string, string> _headers = new Dictionary<string, string>();

    /// <summary>
    /// Sets the base URL to be used for the HTTP requests being built.
    /// </summary>
    /// <param name="uri">The base URL as a string.</param>
    /// <returns>The current instance of <see cref="HttpRequestBuilder"/> to allow method chaining.</returns>
    public HttpRequestBuilder SetBaseUrl(string uri)
    {
        BaseUrl = new Uri(uri);
        return this;
    }

    /// <summary>
    /// Sets the URL to be used for the HTTP requests being built.
    /// </summary>
    /// <param name="uri">The complete URL as a string.</param>
    /// <returns>The current instance of <see cref="HttpRequestBuilder"/> to allow method chaining.</returns>
    public HttpRequestBuilder SetUrl(string uri)
    {
        Url = uri;
        return this;
    }

    /// <summary>
    /// Adds an action to be executed when the HTTP request results in a specified failure status code.
    /// </summary>
    /// <param name="statusCode">The HTTP status code that triggers the specified action.</param>
    /// <param name="func">The function to execute asynchronously when the specified status code is encountered.</param>
    /// <returns>The current instance of <see cref="HttpRequestBuilder"/> to allow method chaining.</returns>
    public HttpRequestBuilder AddOnFailedAction(HttpStatusCode statusCode, Func<Task> func)
    {
        if (!_onFailedApiResultFuncDictionary.TryAdd(statusCode, func))
            _onFailedApiResultFuncDictionary.Add(statusCode, func);

        return this;
    }

    /// <summary>
    /// Configures the timeout duration for the HTTP requests being built.
    /// </summary>
    /// <param name="timeOutInSecond">The timeout duration in seconds.</param>
    /// <returns>The current instance of <see cref="HttpRequestBuilder"/> to allow method chaining.</returns>
    public HttpRequestBuilder SetRequestTimeOut(int timeOutInSecond)
    {
        TimeOutInSecond = timeOutInSecond;
        return this;
    }

    /// <summary>
    /// Configures whether to enforce the HTTP response to have a successful status code.
    /// </summary>
    /// <param name="isActive">A boolean value indicating whether to enforce successful status codes.
    /// When set to true, an exception will be thrown if the response status code is not successful.</param>
    /// <returns>The current instance of <see cref="HttpRequestBuilder"/> to allow method chaining.</returns>
    public HttpRequestBuilder EnsureSuccessStatusCode(bool isActive = true)
    {
        _ensureSuccessStatusCode = isActive;
        return this;
    }

    /// <summary>
    /// Sets the HTTP method to be used for the request being built.
    /// </summary>
    /// <param name="httpMethod">The HTTP method to set, such as GET, POST, PUT, DELETE, etc.</param>
    /// <returns>The current instance of <see cref="HttpRequestBuilder"/> to allow method chaining.</returns>
    public HttpRequestBuilder SetMethod(HttpMethod httpMethod)
    {
        HttpMethod = httpMethod;
        return this;
    }

    /// <summary>
    /// Sets the JSON body for the HTTP request being built.
    /// </summary>
    /// <param name="body">The object to be serialized and used as the JSON body of the request.</param>
    /// <returns>The current instance of <see cref="HttpRequestBuilder"/> to allow method chaining.</returns>
    public HttpRequestBuilder SetJsonBody(object body)
    {
        JsonBody = body;
        return this;
    }

    /// <summary>
    /// Enables the generation of a cURL script representation of the HTTP request being built.
    /// </summary>
    /// <returns>The current instance of <see cref="HttpRequestBuilder"/> to allow method chaining.</returns>
    public HttpRequestBuilder LogCurlScript(ILogger logger, string logKey = "HttpRequestBuilder")
    {
        _generateCurlScript = true;
        _logger = logger;
        _logKey = logKey;
        return this;
    }

    /// <summary>
    /// Specifies that the HTTP requests being built should only accept responses with a JSON content type.
    /// </summary>
    /// <returns>The current instance of <see cref="HttpRequestBuilder"/> to allow method chaining.</returns>
    public HttpRequestBuilder SetAcceptOnlyJson()
    {
        AcceptOnlyJson = true;
        return this;
    }

    /// <summary>
    /// Adds a query parameter to the HTTP request being built.
    /// </summary>
    /// <param name="key">The key of the query parameter.</param>
    /// <param name="value">The value of the query parameter. Can be null.</param>
    /// <returns>The current instance of <see cref="HttpRequestBuilder"/> to allow method chaining.</returns>
    public HttpRequestBuilder AddQueryParam(string key, string? value)
    {
        if (value != null)
            _queryParams.Add(key, value);
        return this;
    }

    /// <summary>
    /// Adds a query parameter to the HTTP request being built, using an object value.
    /// </summary>
    /// <param name="key">The key of the query parameter.</param>
    /// <param name="value">The value of the query parameter as an object. Can be null.</param>
    /// <returns>The current instance of <see cref="HttpRequestBuilder"/> to allow method chaining.</returns>
    public HttpRequestBuilder AddQueryParam(string key, object? value)
    {
        if (value != null)
            AddQueryParam(key, value.ToString()!);
        return this;
    }

    /// <summary>
    /// Adds a query parameter to the HTTP request being built. extract the property name and value
    /// </summary>
    /// <param name="obj">The key of the query parameter.</param>
    /// <returns>The current instance of <see cref="HttpRequestBuilder"/> to allow method chaining.</returns>
    public HttpRequestBuilder AddQueryParam(object obj)
    {
        foreach (PropertyInfo pi in obj.GetType().GetProperties())
            AddQueryParam(pi.Name, pi.GetValue(obj, null)!);
        return this;
    }

    /// <summary>
    /// Adds a header to the HTTP request being built.
    /// </summary>
    /// <param name="key">The name of the header.</param>
    /// <param name="value">The value of the header.</param>
    /// <returns>The current instance of <see cref="HttpRequestBuilder"/> to allow method chaining.</returns>
    public HttpRequestBuilder AddHeader(string key, object value)
    {
        _headers.Add(key, value.ToString()!);
        return this;
    }

    /// <summary>
    /// Adds a single header to the HTTP request being built.
    /// </summary>
    /// <param name="header">A key-value pair representing the header to add, where the key is the header name and the value is the header value.</param>
    /// <returns>The current instance of <see cref="HttpRequestBuilder"/> to allow method chaining.</returns>
    public HttpRequestBuilder AddHeader(KeyValuePair<string, string> header)
    {
        _headers.Add(header.Key, header.Value);
        return this;
    }

    /// <summary>
    /// Adds a form parameter to the request body (for application/x-www-form-urlencoded).
    /// </summary>
    /// <param name="key">The name of the form parameter.</param>
    /// <param name="value">The value of the form parameter.</param>
    /// <returns>The current instance of <see cref="HttpRequestBuilder"/> to allow method chaining.</returns>
    public HttpRequestBuilder AddFormParam(string key, string value)
    {
        _formParameters.Add(key, value);
        return this;
    }

    /// <summary>
    /// Adds a form parameter to be included in the request body (for application/x-www-form-urlencoded).
    /// </summary>
    /// <param name="key">The key of the form parameter.</param>
    /// <param name="value">The value of the form parameter.</param>
    /// <returns>The current instance of <see cref="HttpRequestBuilder"/> to allow method chaining.</returns>
    public HttpRequestBuilder AddFormParam(string key, object value)
    {
        var valueInString = value.ToString();
        if (string.IsNullOrWhiteSpace(valueInString))
            return this;

        _formParameters.Add(key, valueInString);
        return this;
    }

    /// <summary>
    /// Adds a file to the multipart form data content (for file uploads).
    /// </summary>
    /// <param name="name">The name of the form field.</param>
    /// <param name="fileStream">The stream containing the file data.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <param name="contentType">The MIME type of the file (optional).</param>
    /// <returns>The current instance of <see cref="HttpRequestBuilder"/> to allow method chaining.</returns>
    public HttpRequestBuilder AddFormFile(string name, Stream fileStream, string fileName, string? contentType = null)
    {
        _multipartContent ??= new MultipartFormDataContent();

        var streamContent = new StreamContent(fileStream);
        if (!string.IsNullOrWhiteSpace(contentType))
        {
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        }

        _multipartContent.Add(streamContent, name, fileName);
        return this;
    }

    /// <summary>
    /// Adds a file to the multipart form data content using IFormFile (for file uploads).
    /// </summary>
    /// <param name="name">The name of the form field.</param>
    /// <param name="formFile">The IFormFile to upload.</param>
    /// <returns>The current instance of <see cref="HttpRequestBuilder"/> to allow method chaining.</returns>
    public HttpRequestBuilder AddFormFile(string name, Microsoft.AspNetCore.Http.IFormFile formFile)
    {
        var stream = formFile.OpenReadStream();
        return AddFormFile(name, stream, formFile.FileName, formFile.ContentType);
    }

    /// <summary>
    /// Adds a string value to the multipart form data content (used with file uploads).
    /// </summary>
    /// <param name="name">The name of the form field.</param>
    /// <param name="value">The string value.</param>
    /// <returns>The current instance of <see cref="HttpRequestBuilder"/> to allow method chaining.</returns>
    public HttpRequestBuilder AddMultipartFormParam(string name, string value)
    {
        _multipartContent ??= new MultipartFormDataContent();
        _multipartContent.Add(new StringContent(value), name);
        return this;
    }

    /// <summary>
    /// Builds an HTTP request message based on the configured properties of the current instance.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>An <see cref="HttpRequestMessage"/> containing the configured request data.</returns>
    public async Task<HttpRequestMessage> BuildHttpRequestMessage(CancellationToken cancellationToken)
    {
        var uriBuilder = httpClient.BaseAddress is not null
            ? new UriBuilder(new Uri(httpClient.BaseAddress, Url))
            : new UriBuilder(new Uri(BaseUrl, Url));

        if (_queryParams.Any())
        {
            using var content = new FormUrlEncodedContent(_queryParams
                .Select(c => new KeyValuePair<string, string>(c.Key, c.Value)));
            uriBuilder.Query = await content.ReadAsStringAsync(cancellationToken);
        }

        var httpRequestMessage = new HttpRequestMessage
        {
            Method = HttpMethod,
            RequestUri = uriBuilder.Uri
        };

        if (AcceptOnlyJson)
        {
            httpRequestMessage.Headers.Accept.Clear();
            httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        foreach (var (key, value) in _headers)
            httpRequestMessage.Headers.Add(key, value);

        if (_multipartContent is not null)
        {
            httpRequestMessage.Content = _multipartContent;
        }
        else if (JsonBody is not null)
        {
            httpRequestMessage.Content = JsonContent.Create(JsonBody, options: new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
        else if (_formParameters.Any())
        {
            httpRequestMessage.Content = new FormUrlEncodedContent(_formParameters);
        }

        return httpRequestMessage;
    }

    /// <summary>
    /// Sends an HTTP request asynchronously and deserializes the JSON response into the specified type.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <typeparam name="TResult">The type to which the JSON response will be deserialized.</typeparam>
    /// <returns>An instance of <typeparamref name="TResult"/> representing the deserialized JSON response.</returns>
    public async Task<TResult> FromJsonAsync<TResult>(CancellationToken cancellationToken)
    {
        var stringResponse = await FromStringAsync(cancellationToken);

        return JsonConvert.DeserializeObject<TResult>(stringResponse)!;
    }

    /// <summary>
    /// Executes the HTTP request and retrieves the response as a string asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the HTTP response body as a string.</returns>
    public async Task<string> FromStringAsync(CancellationToken cancellationToken)
    {
        var httpResponseMessage = TimeOutInSecond.HasValue
            ? await SendWithTimeoutAsync(cancellationToken)
            : await SendAsync(cancellationToken);

        var stringResponse = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);

        return stringResponse;
    }

    /// <summary>
    /// Executes the HTTP request and retrieves the response as a strongly-typed <see cref="HttpResult"/> instance.
    /// </summary>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <typeparam name="TResult">The type to which the response content will be deserialized.</typeparam>
    /// <returns>A task that represents the asynchronous operation. The task result is a <see cref="HttpResult{TResult}"/> containing the response data and status.</returns>
    public async Task<HttpResult<TResult>> FromHttpResultAsync<TResult>(CancellationToken ct)
        where TResult : class
    {
        var httpResponseMessage = TimeOutInSecond.HasValue
            ? await SendWithTimeoutAsync(ct)
            : await SendAsync(ct);

        return await HttpResult<TResult>.FromHttpResponseAsync(httpResponseMessage);
    }

    /// <summary>
    /// Executes the HTTP request and returns a tuple containing the HTTP status code
    /// and the response content as a string.
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel the operation if necessary.</param>
    /// <returns>A tuple consisting of <see cref="HttpStatusCode"/> and the response content as a string.</returns>
    public async Task<(HttpStatusCode StatusCode, string Result)> FromStringWithStatusCodeAsync(
        CancellationToken cancellationToken)
    {
        var httpResponseMessage = TimeOutInSecond.HasValue
            ? await SendWithTimeoutAsync(cancellationToken)
            : await SendAsync(cancellationToken);

        var stringResponse = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);

        return (httpResponseMessage.StatusCode, stringResponse);
    }

    private async Task<HttpResponseMessage> SendWithTimeoutAsync(CancellationToken ct)
    {
        if (!TimeOutInSecond.HasValue)
            throw new NullReferenceException(nameof(TimeOutInSecond));

        async Task<HttpResponseMessage> TimeoutFuncTask()
        {
            await Task.Delay(TimeSpan.FromSeconds(TimeOutInSecond!.Value), ct);
            throw new Exception(
                $"request timed out in {TimeOutInSecond!.Value} second. [RequestUrl]=({BaseUrl + Url}) ," +
                $" [RequestBody]=({JsonConvert.SerializeObject(JsonBody)})");
        }

        var timeoutTask = Task.Run((Func<Task<HttpResponseMessage>>)TimeoutFuncTask, ct);

        var httpRequestMessage = await BuildHttpRequestMessage(ct);
        if (_generateCurlScript)
        {
            var curlScript = httpClient.GenerateCurlInString(httpRequestMessage);
            _logger.Information($"{_logKey}: (Curl Script)=[{curlScript}]");
        }

        var httpRequestTask = httpClient.SendAsync(httpRequestMessage, ct);
        var completedTask = await Task.WhenAny(timeoutTask, httpRequestTask);
        var httpResponse = await completedTask;

        if (!httpResponse.IsSuccessStatusCode &&
            _onFailedApiResultFuncDictionary.TryGetValue(httpResponse.StatusCode, out var task))
            await task();

        return httpResponse;
    }

    private async Task<HttpResponseMessage> SendAsync(CancellationToken ct)
    {
        var httpRequestMessage = await BuildHttpRequestMessage(ct);

        if (_generateCurlScript)
        {
            var curlScript = httpClient.GenerateCurlInString(httpRequestMessage);
            _logger.Information($"{_logKey}: (Curl Script)=[{{CurlScript}}]", curlScript);
        }

        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, ct);

        if (_ensureSuccessStatusCode)
            httpResponseMessage.EnsureSuccessStatusCode();

        if (!httpResponseMessage.IsSuccessStatusCode &&
            _onFailedApiResultFuncDictionary.TryGetValue(httpResponseMessage.StatusCode, out var onFailedFunc))
            await onFailedFunc();

        return httpResponseMessage;
    }
}