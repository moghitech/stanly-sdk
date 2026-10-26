using Fusion.Storage.Extensions;
using Microsoft.AspNetCore.Builder;

namespace Fusion.Microservice.Extensions.Storage;

/// <summary>
/// WebApplicationBuilder extensions for object storage registration setups.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Configures default object storage. 
    /// </summary>
    /// <param name="builder"></param>
    public static void UseObjectStorage(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;

        builder.Services.AddS3ObjectStorageServices(configuration);
    }
}