using System.Diagnostics;
using Fusion.Client.Metrics;

namespace Fusion.Client.Handlers;

internal class ProxyMetricsHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        HttpResponseMessage? response = null;
        try
        {
            response = await base.SendAsync(request, cancellationToken);
            return response;
        }
        finally
        {
            stopwatch.Stop();
            var statusCode = ((int?)response?.StatusCode)?.ToString() ?? "0";
            var endpoint = request.RequestUri?.AbsolutePath ?? string.Empty;

            ProxyMetrics.ApiCallDuration
                .WithLabels(request.Method.Method, endpoint)
                .Observe(stopwatch.Elapsed.TotalMilliseconds);

            ProxyMetrics.ApiResponseStatus
                .WithLabels(request.Method.Method, endpoint, statusCode)
                .Inc();
        }
    }
}