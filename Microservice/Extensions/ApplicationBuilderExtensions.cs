using Fusion.Microservice.Extensions.Sentry;
using Fusion.Microservice.Extensions.Serilog;
using Microsoft.AspNetCore.Builder;

namespace Fusion.Microservice.Extensions;

/// <summary>
/// Provides extension methods for customizing the behavior of the application builder
/// with additional middleware components or service integrations specific to the application's requirements.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Configures the application to use custom services like Sentry and Serilog
    /// by creating middleware components or service integrations
    /// based on the application's configuration.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="WebApplicationBuilder"/> for configuring the application.
    /// </param>
    public static void UseCustomServices(this WebApplicationBuilder builder)
    {
        var options = builder.Configuration.GetServiceOptions();

        builder.UseCustomSentry(options.SentryOptions);
        builder.UseCustomSerilog();
    }
}