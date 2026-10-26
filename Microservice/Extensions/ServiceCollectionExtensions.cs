using System.Reflection;
using System.Text.Json.Serialization;
using Fusion.FluentValidation.Extensions;
using Fusion.Microservice.Converters;
using Fusion.Microservice.Domain;
using Fusion.Microservice.Extensions.ApiVersion;
using Fusion.Microservice.Extensions.OpenTelemetry;
using Fusion.Microservice.Extensions.Sentry;
using Fusion.Microservice.Extensions.Swagger;
using Fusion.Microservice.Filters;
using Fusion.Microservice.Helpers;
using Fusion.Microservice.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;

namespace Fusion.Microservice.Extensions;

/// <summary>
/// Microservice Service Collection Extensions
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers common services and configurations for the application including
    /// controllers, JSON options, health checks, memory caching, Swagger, Sentry,
    /// OpenTelemetry, validation, API versioning, middleware, and HttpClient
    /// dependencies.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> instance used to configure the application services.</param>
    public static void RegisterCommonServices(this WebApplicationBuilder builder)
    {
        var options = builder.Configuration.GetServiceOptions();

        EnvironmentVariableHelpers.SetServiceName(options.ServiceName);

        builder.Services.AddResponseCompression(op =>
        {
            op.EnableForHttps = true;
            op.Providers.Add<GzipCompressionProvider>();
        });

        builder.Services.AddControllers(o => { o.Filters.Add<ApiResultFilter>(); }).AddJsonOptions(j =>
        {
            j.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            j.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
            j.JsonSerializerOptions.Converters.Add(new TimeSpanJsonConverter());
        }).ConfigureApiBehaviorOptions(setup =>
        {
            setup.InvalidModelStateResponseFactory = action =>
            {
                var validationErrors = action.ModelState
                    .ToDictionary(r => r.Key,
                        r => r.Value?.Errors
                            .Select(q => q.ErrorMessage)
                            .ToList() ?? ["[Unknown]"]
                    );

                var result = new ApiResult
                {
                    IsSuccess = false,
                    Message = "Validation failed",
                    ValidationErrors = validationErrors
                };

                return new ObjectResult(result)
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            };
        });

        builder.Services.AddHealthChecks();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddMemoryCache();

        builder.Services.AddCustomSwagger(options.AuthenticationOptions.IsEnabled);
        builder.Services.AddCustomSentry(options.SentryOptions);
        builder.Services.AddOpenTelemetryServices(builder.Environment, options);

        builder.Services.AddValidation(Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly());

        builder.Services.AddApiVersioningService();

        builder.Services.AddScoped<ExceptionHandlingMiddleware>();
        builder.Services.AddExceptionHandler<ExceptionHandlingMiddleware>();
        builder.Services.AddProblemDetails();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddHttpClient();
    }
}