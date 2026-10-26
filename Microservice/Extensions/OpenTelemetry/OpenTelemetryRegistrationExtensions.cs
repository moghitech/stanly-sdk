using Fusion.Exceptions;
using Fusion.Microservice.Extensions.AppVersion;
using Fusion.Microservice.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Fusion.Microservice.Extensions.OpenTelemetry;

internal static class OpenTelemetryRegistrationExtensions
{
    public static void AddOpenTelemetryServices(this IServiceCollection services, IWebHostEnvironment environment,
        ServiceOptions options)
    {
        if (!options.OpenTelemetry.IsEnabled)
            return;

        if (options.OpenTelemetry is null || string.IsNullOrWhiteSpace(options.OpenTelemetry.Endpoint))
            throw new RuntimeException("OTLP endpoint is not configured");

        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(
                    serviceName: options.ServiceName,
                    serviceVersion: AppVersionAccessor.GetAppVersion(),
                    serviceInstanceId: Environment.MachineName)
                .AddAttributes([
                    new KeyValuePair<string, object>("deployment.environment", environment.EnvironmentName),
                    new KeyValuePair<string, object>("host.name", Environment.MachineName)
                ])
            ).WithTracing(t => t
                .AddAspNetCoreInstrumentation(o =>
                {
                    o.RecordException = true;
                    o.EnrichWithHttpRequest = (activity, httpRequest) =>
                    {
                        activity.SetTag("http.request.size", httpRequest.ContentLength);
                        if (string.IsNullOrWhiteSpace(httpRequest.HttpContext.TraceIdentifier))
                            return;
                        activity.SetTag("request.id", httpRequest.HttpContext.TraceIdentifier);
                        activity.SetTag("http.request.user_agent", httpRequest.Headers.UserAgent.ToString());
                    };
                    o.EnrichWithHttpResponse = (activity, httpResponse) =>
                    {
                        activity.SetTag("http.response.size", httpResponse.ContentLength);
                    };
                })
                .AddHttpClientInstrumentation(o =>
                {
                    o.RecordException = true;
                    o.EnrichWithHttpRequestMessage = (activity, httpRequestMessage) =>
                    {
                        activity.SetTag("http.request.method", httpRequestMessage.Method.ToString());
                    };
                    o.EnrichWithHttpResponseMessage = (activity, httpResponseMessage) =>
                    {
                        activity.SetTag("http.response.status_code", (int)httpResponseMessage.StatusCode);
                    };
                })
                .AddNpgsql()
                .AddSqlClientInstrumentation(o =>
                {
                    o.SetDbStatementForText = true;
                    o.RecordException = true;
                    o.EnableConnectionLevelAttributes = true;
                })
                .AddEntityFrameworkCoreInstrumentation(efOptions =>
                {
                    efOptions.SetDbStatementForText = true;
                    efOptions.EnrichWithIDbCommand = (activity, command) =>
                    {
                        activity.SetTag("db.command_timeout", command.CommandTimeout);
                    };
                })
                .AddAWSInstrumentation()
                .AddRedisInstrumentation(o => { o.SetVerboseDatabaseStatements = true; })
                .AddSource("Wolverine")
                .AddSource("RabbitMQ.Client.*")
                .AddOtlpExporter(o =>
                {
                    o.Endpoint = new Uri(options.OpenTelemetry.Endpoint);
                    o.Protocol = OtlpExportProtocol.Grpc;
                }))
            .WithMetrics(m => m
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddProcessInstrumentation()
                .AddOtlpExporter(o =>
                {
                    o.Endpoint = new Uri(options.OpenTelemetry.Endpoint);
                    o.Protocol = OtlpExportProtocol.Grpc;
                }));
    }
}