using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Fusion.Microservice.Extensions.Swagger.Filters;

/// <summary>
/// Swagger operation filter that conditionally adds an optional <c>CUSTOMER-ID</c> header
/// parameter to API operations. The header is only added for Swagger documents whose
/// name ends with <c>-customer-id</c>.
/// </summary>
public class CustomerIdHeaderFilter : IOperationFilter
{
    /// <summary>
    /// Applies the filter to add the optional <c>CUSTOMER-ID</c> header parameter
    /// when the current Swagger document name indicates customer-based APIs.
    /// </summary>
    /// <param name="operation">The Swagger operation being processed.</param>
    /// <param name="context">The context for the operation filter, including schema and document info.</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();

        if (!context.DocumentName.EndsWith("-customer-id", StringComparison.OrdinalIgnoreCase))
            return;

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "CUSTOMER-ID",
            In = ParameterLocation.Header,
            Description = "Optional customer identifier",
            Required = false,
            Schema = new OpenApiSchema { Type = "string" }
        });
    }
}