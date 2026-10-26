using System.Net.Mime;
using System.Text.Json;
using Fusion.Microservice.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Fusion.Microservice.Extensions.Authorization;

/// <summary>
/// Provides extensions for registering authorization-related configuration and functionality
/// within an application's routing system.
/// </summary>
public static class AuthorizationServiceRegistration
{
    /// <summary>
    /// Configures the application to use JWT-based authentication using the default
    /// <see cref="JwtBearerDefaults.AuthenticationScheme"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the authentication services to.</param>
    public static void AddReadOnlyJwtAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddScheme<AuthenticationSchemeOptions, JwtRoleOnlyAuthenticationHandler>(
                JwtBearerDefaults.AuthenticationScheme, _ => { });
    }

    /// <summary>
    /// Adds authorization-related endpoints to the application's route configuration.
    /// This includes an endpoint to retrieve the roles defined in the authorization options.
    /// </summary>
    /// <param name="app">The <see cref="IEndpointRouteBuilder"/> to configure the endpoints on.</param>
    /// <param name="authorizationAction">A delegate used to configure the <see cref="AuthorizationOptions"/>.</param>
    public static void AddAuthorization(this IEndpointRouteBuilder app,
        Action<AuthorizationOptions> authorizationAction)
    {
        var options = new AuthorizationOptions();
        authorizationAction.Invoke(options);

        app.MapGet("/_roles", async context =>
        {
            context.Response.ContentType = MediaTypeNames.Application.Json;
            var response = new { roles = options.Roles };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        });
    }
}