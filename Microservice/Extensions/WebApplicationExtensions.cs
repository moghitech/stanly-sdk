using Fusion.Microservice.Extensions.AppVersion;
using Fusion.Microservice.Extensions.HealthCheck;
using Fusion.Microservice.Extensions.Liveness;
using Fusion.Microservice.Extensions.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Prometheus;

namespace Fusion.Microservice.Extensions;

/// <summary>
/// Microservice WebApplication Extensions
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Configures common middlewares and features for the application, including exception handling,
    /// metrics, swagger, HTTPS redirection, CORS, authentication, authorization, health checks,
    /// and app version information.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication"/> instance where the middlewares and features will be configured.</param>
    /// <param name="corsPolicy">
    /// The name of the CORS policy to apply. If null, a default CORS policy will be used.
    /// </param>
    /// <param name="useAuth">
    /// A boolean value that specifies whether authentication and authorization should be enabled.
    /// </param>
    public static void UseCommon(this WebApplication app, string? corsPolicy = null, bool useAuth = false)
    {
        app.UseExceptionHandler();
        app.UseHttpMetrics();
        app.UseMetricServer();

        if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
            app.UseCustomSwagger();

        app.UseHttpsRedirection();

        app.UseRouting();

        if (corsPolicy is null)
            app.UseCors();
        else
            app.UseCors(corsPolicy);

        if (useAuth)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

        app.UseHealthCheck();
        app.UseLiveness();
        app.UseAppVersion();

        app.MapControllers();
    }
}