using Fusion.Microservice.Extensions.Swagger.Filters;
using Fusion.Microservice.Extensions.Swagger.Options;
using Fusion.Microservice.Helpers;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;
using Unchase.Swashbuckle.AspNetCore.Extensions.Options;

namespace Fusion.Microservice.Extensions.Swagger;

/// <summary>
/// IServiceCollection extension methods for Swagger setup
/// </summary>
internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// customize swagger include: versioning by url, jwt config
    /// </summary>
    /// <param name="services"></param>
    /// <param name="hasAuthorizedEndpoints">For gateways, it must be "true"</param>
    internal static void AddCustomSwagger(this IServiceCollection services, bool hasAuthorizedEndpoints = false)
    {
        services.AddSwaggerGen(c =>
        {
            var serviceName = EnvironmentVariableHelpers.GetServiceName();

            c.SwaggerDoc(serviceName, new OpenApiInfo { Title = $"{serviceName} API document", Version = serviceName });

            c.SwaggerDoc($"{serviceName}-customer-id", new OpenApiInfo
            {
                Title = $"{serviceName} API document (with CUSTOMER-ID)",
                Version = $"{serviceName}-customer-id"
            });

            if (hasAuthorizedEndpoints)
            {
                var securitySchema = new OpenApiSecurityScheme
                {
                    Description =
                        "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "bearer",
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                };
                c.AddSecurityDefinition("Bearer", securitySchema);

                var securityRequirement = new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                            { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
                        Array.Empty<string>()
                    }
                };
                c.AddSecurityRequirement(securityRequirement);
            }

            c.AddEnumsWithValuesFixFilters(o =>
            {
                // add descriptions from DescriptionAttribute or xml-comments to fix enums (add 'x-enumDescriptions' or its alias from XEnumDescriptionsAlias for schema extensions) for applied filters
                o.IncludeDescriptions = true;

                // get descriptions from DescriptionAttribute then from xml-comments
                o.DescriptionSource = DescriptionSources.DescriptionAttributesThenXmlComments;

                var xmlFiles = Directory
                    .GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly)
                    .ToList();

                // get descriptions from xml-file comments on the specified path
                xmlFiles.ForEach(xmlFile => { o.IncludeXmlCommentsFrom(xmlFile); });
            });

            c.DocInclusionPredicate((_, _) => true);
            c.CustomSchemaIds(GetInnermostGenericTypeFullName);
            c.OperationFilter<CustomerIdHeaderFilter>();
        });

        services.AddEndpointsApiExplorer();
        services.ConfigureOptions<ConfigureSwaggerOptions>();
        services.AddFluentValidationRulesToSwagger();
    }

    private static string GetInnermostGenericTypeFullName(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        if (!type.IsGenericType)
            return type.FullName!;

        List<string> genericTypes = [];
        while (type.IsGenericType)
        {
            var genericArguments = type.GetGenericArguments();
            if (genericArguments.Length > 0)
            {
                genericTypes.Add(type.Name.Replace("`1", string.Empty));
                type = genericArguments[0];
            }
            else
                break;
        }

        return $"{string.Join(".", genericTypes)}.{type.FullName}";
    }
}