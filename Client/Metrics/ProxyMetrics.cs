using System.Reflection;
using Prometheus;

namespace Fusion.Client.Metrics;

internal static class ProxyMetrics
{
    private static readonly string? ProjectName =
        Assembly.GetEntryAssembly()?.GetName().Name?.Replace(".", "_").ToLower();

    internal static readonly Histogram ApiCallDuration = Prometheus.Metrics
        .CreateHistogram($"{ProjectName}_api_call_duration_seconds", "Duration of API calls in milliseconds",
            new HistogramConfiguration
            {
                LabelNames = new[] { "method", "endpoint" },
                Buckets = Histogram.LinearBuckets(start: 0.1, width: 0.1, count: 10)
            });

    internal static readonly Counter ApiResponseStatus = Prometheus.Metrics
        .CreateCounter($"{ProjectName}_api_response_status_total", "Total number of API responses by status code",
            new CounterConfiguration
            {
                LabelNames = new[] { "method", "endpoint", "status_code" }
            });
}