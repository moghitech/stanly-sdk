using System.Net;
using System.Net.Http.Headers;
using Fusion.Client.Abstractions;
using Fusion.Client.Constants;
using Fusion.Client.Handlers;
using Fusion.Microservice.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Refit;

namespace Fusion.Client.Extensions;

/// <summary>
/// IServiceCollection extension methods for HttpClient setup
/// </summary>
public static class HttpClientExtensions
{
    /// <summary>
    /// Configures and sets up a proxy client for HTTP communication with retry and circuit breaker policies,
    /// adding authentication headers, metrics handlers, and base configurations.
    /// </summary>
    /// <typeparam name="TProxy">The type of the proxy client interface to be configured.</typeparam>
    /// <param name="services">The service collection to which the proxy client and dependencies will be added.</param>
    /// <param name="clientUri">The ClientUri object used to calculate the base URI of the proxy client.</param>
    public static void SetupProxyClient<TProxy>(this IServiceCollection services, ClientUri clientUri)
        where TProxy : class, IProxy
    {
        var serviceName = EnvironmentVariableHelpers.GetServiceName();
        var environmentName = EnvironmentVariableHelpers.GetEnvironmentName();

        services.AddTransient<ProxyMetricsHandler>();
        services.AddTransient<ProxyLoggingHandler>();

        services.AddRefitClient<TProxy>(new RefitSettings
            {
                ContentSerializer = new CustomContentSerializer(),
            }).ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
            {
                KeepAlivePingDelay = TimeSpan.FromMinutes(1),
                KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
                PooledConnectionLifetime = TimeSpan.FromMinutes(10),
                PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2),
                MaxConnectionsPerServer = 1024,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                EnableMultipleHttp2Connections = true
            }).ConfigureHttpClient((_, c) =>
            {
                c.BaseAddress = new Uri(clientUri.CalculateUri());
                c.DefaultRequestVersion = HttpVersion.Version20;
                c.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
                c.Timeout = TimeSpan.FromSeconds(300);
                c.DefaultRequestHeaders.Add(HttpRequestHeaderConstants.FromService, serviceName);
                c.DefaultRequestHeaders.Add(HttpRequestHeaderConstants.FromServiceEnvironment, environmentName);
                c.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            }).AddPolicyHandler((_, _) => Policy<HttpResponseMessage>
                .HandleResult(CheckCanRetry)
                .WaitAndRetryAsync(
                    HttpClientConstants.RetryCount,
                    retryAttempt =>
                        TimeSpan.FromMilliseconds(HttpClientConstants.BaseRetryDelayInMillisecond * retryAttempt))
            ).AddPolicyHandler((_, _) => GetCircuitPolicy())
            .AddHttpMessageHandler<ProxyLoggingHandler>()
            .AddHttpMessageHandler<ProxyMetricsHandler>();
    }

    private static AsyncCircuitBreakerPolicy<HttpResponseMessage> GetCircuitPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                HttpClientConstants.CircuitBreakerHandledEventsAllowedBeforeBreaking,
                TimeSpan.FromSeconds(HttpClientConstants.DurationOfBreakInSecond)
            );
    }

    private static bool CheckCanRetry(HttpResponseMessage response)
    {
        var allowedForRetryStatusCodes = new List<HttpStatusCode>()
        {
            HttpStatusCode.GatewayTimeout,
            HttpStatusCode.RequestTimeout,
            HttpStatusCode.TooManyRequests,
            HttpStatusCode.InternalServerError,
            HttpStatusCode.BadGateway,
            HttpStatusCode.ServiceUnavailable
        };
        return !response.IsSuccessStatusCode && (allowedForRetryStatusCodes.Contains(response.StatusCode) ||
                                                 response.StatusCode < HttpStatusCode.BadRequest ||
                                                 response.StatusCode >= HttpStatusCode.InternalServerError);
    }
}