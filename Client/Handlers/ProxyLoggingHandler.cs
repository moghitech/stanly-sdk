using Fusion.Client.Constants;
using Fusion.Exceptions.BusinessExceptions;
using Fusion.Microservice.Domain;
using Microsoft.Extensions.Logging;

namespace Fusion.Client.Handlers;

/// <summary>
/// A custom <see cref="DelegatingHandler"/> that logs failed proxy HTTP requests,
/// including contextual information such as service name, environment, and customer ID.
/// </summary>
public sealed class ProxyLoggingHandler : DelegatingHandler
{
    private readonly ILogger<ProxyLoggingHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProxyLoggingHandler"/> class
    /// with the specified logger.
    /// </summary>
    /// <param name="logger">The logger used to record request and response information.</param>
    public ProxyLoggingHandler(ILogger<ProxyLoggingHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Sends an HTTP request to the inner handler and logs detailed error information
    /// if the response indicates a failure (non-success status code).
    /// </summary>
    /// <param name="request">The HTTP request message to send.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>
    /// The HTTP response message returned by the inner handler.
    /// </returns>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);
        if (response.IsSuccessStatusCode)
            return response;

        var requestUrl = request.RequestUri?.ToString() ?? "unknown";
        var requestMethod = request.Method.ToString();

        var fromService = "unknown";
        var fromEnv = "unknown";
        var customerId = "none";

        if (request.Headers.TryGetValues(HttpRequestHeaderConstants.FromService, out var fromServiceValues))
            fromService = fromServiceValues.FirstOrDefault() ?? "unknown";

        if (request.Headers.TryGetValues(HttpRequestHeaderConstants.FromServiceEnvironment, out var fromEnvValues))
            fromEnv = fromEnvValues.FirstOrDefault() ?? "unknown";

        if (request.Headers.TryGetValues(HttpRequestHeaderConstants.CustomerId, out var customerIdValues))
            customerId = customerIdValues.FirstOrDefault() ?? "none";

        var errorResult = await response.Content.ReadAsAsync<ApiResult>(cancellationToken);

        _logger.LogError(
            "{LogKey} API CALL FAILED: [Method]=({Method}), [URL]=({Url}), [Status]=({StatusCode}), [Reason]=({ReasonPhrase}), [FromService]=({FromService}), [FromEnv]=({FromEnv}), [CustomerId]=({CustomerId}).",
            nameof(ProxyLoggingHandler),
            requestMethod,
            requestUrl,
            (int)response.StatusCode,
            errorResult?.Message,
            fromService,
            fromEnv,
            customerId
        );

        var message = errorResult?.Message ?? "Unknown Error";
        throw new BusinessException(message, message, response.StatusCode);
    }
}