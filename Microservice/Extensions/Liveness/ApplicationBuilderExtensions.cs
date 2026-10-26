using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Fusion.Microservice.Extensions.Liveness;

/// <summary>
/// Provides extension methods for configuring liveness-related middleware in an application.
/// </summary>
internal static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Configures a liveness endpoint in the application pipeline.
    /// </summary>
    /// <param name="app">The endpoint route builder instance used to configure the liveness route.</param>
    internal static void UseLiveness(this IEndpointRouteBuilder app)
    {
        app.MapGet("/live", async context =>
        {
            context.Response.ContentType = MediaTypeNames.Application.Json;
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { status = "ok" }));
        });
    }
}