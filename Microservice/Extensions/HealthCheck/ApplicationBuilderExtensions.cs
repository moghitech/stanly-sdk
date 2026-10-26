using System.Net;
using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace Fusion.Microservice.Extensions.HealthCheck;

/// <summary>
/// Provides extension methods for configuring health checks in the application pipeline.
/// </summary>
internal static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Configures the health check middleware in the application pipeline.
    /// </summary>
    /// <param name="app">The application builder instance used to configure the middleware pipeline.</param>
    public static void UseHealthCheck(this IApplicationBuilder app)
    {
        app.UseHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                try
                {
                    var environment = context.RequestServices.GetRequiredService<IHostEnvironment>();
                    var result = JsonSerializer.Serialize(
                        new
                        {
                            version = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TAG"))
                                ? "v1.0.0"
                                : Environment.GetEnvironmentVariable("TAG"),
                            enviroment = environment.EnvironmentName,
                            status = report.Status.ToString(),
                            monitors = report.Entries.Select(e =>
                                new { key = e.Key, value = Enum.GetName(typeof(HealthStatus), e.Value.Status) })
                        });
                    context.Response.ContentType = MediaTypeNames.Application.Json;
                    await context.Response.WriteAsync(result);
                }
                catch (Exception e)
                {
                    context.Response.ContentType = MediaTypeNames.Application.Json;
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    await context.Response.WriteAsync($"Bad Health Check : {e.Message}");
                }
            }
        });
    }
}