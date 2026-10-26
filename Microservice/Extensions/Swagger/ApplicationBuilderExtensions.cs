using Fusion.Microservice.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Fusion.Microservice.Extensions.Swagger;

/// <summary>
/// IApplicationBuilder extensions for Swagger middlewares setup
/// </summary>
internal static class ApplicationBuilderExtensions
{
    /// <summary>
    /// application middleware for swagger
    /// </summary>
    /// <param name="app"></param>
    public static void UseCustomSwagger(this IApplicationBuilder app)
    {
        app.UseSwagger();

        app.UseSwaggerUI(options =>
        {
            var serviceName = EnvironmentVariableHelpers.GetServiceName();
            options.DocumentTitle = $"Novin System - {serviceName} API document";
            options.DocExpansion(DocExpansion.None);
            options.DisplayRequestDuration();
            options.EnableDeepLinking();
            options.EnableFilter();
            options.ShowExtensions();
            options.DefaultModelRendering(ModelRendering.Model);
            options.ShowCommonExtensions();
            options.EnableValidator();

            // make a group name list for swagger ui
            using var scope = app.ApplicationServices.CreateScope();
            var provider = scope.ServiceProvider.GetRequiredService<IApiVersionDescriptionProvider>();

            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint(
                    $"/swagger/{description.GroupName}/swagger.json",
                    $"{serviceName}_{description.GroupName.ToUpperInvariant()}"
                );

                options.SwaggerEndpoint(
                    $"/swagger/{serviceName}-customer-id/swagger.json",
                    $"{serviceName}_{description.GroupName.ToUpperInvariant()}_With_CustomerId"
                );
            }
        });
    }
}