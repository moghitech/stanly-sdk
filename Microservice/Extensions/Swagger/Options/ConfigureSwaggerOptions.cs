using Fusion.Microservice.Helpers;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;

namespace Fusion.Microservice.Extensions.Swagger.Options;

internal class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    : IConfigureNamedOptions<SwaggerGenOptions>
{
    /// <summary>
    /// Configure each API discovered for Swagger Documentation
    /// </summary>
    /// <param name="options"></param>
    public void Configure(SwaggerGenOptions options)
    {
        var serviceName = EnvironmentVariableHelpers.GetServiceName();
        var title = $"{serviceName} API";

        // add swagger document for every API version discovered
        foreach (var description in provider.ApiVersionDescriptions.DistinctBy(x => x.GroupName))
        {
            options.SwaggerDoc(
                description.GroupName,
                new OpenApiInfo
                {
                    Title = title,
                    Version = description.GroupName,
                    Description = description.IsDeprecated
                        ? "This API version has been deprecated. Please use new versions."
                        : string.Empty
                }
            );
        }

        var xmlFiles = Directory
            .GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly)
            .ToList();
        xmlFiles.ForEach(xmlFile =>
        {
            options.IncludeXmlComments(xmlFile);
            options.IncludeXmlCommentsWithRemarks(xmlFile, true);
        });
    }

    /// <summary>
    /// Configure Swagger Options. Inherited from the Interface
    /// </summary>
    /// <param name="name"></param>
    /// <param name="options"></param>
    public void Configure(string? name, SwaggerGenOptions options)
    {
        Configure(options);
    }
}