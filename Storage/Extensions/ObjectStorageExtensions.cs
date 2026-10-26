using Fusion.Storage.Configuration;
using Fusion.Storage.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fusion.Storage.Extensions;

/// <summary>
/// The all configuration we need for object storage services. 
/// </summary>
public static class ObjectStorageExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void AddS3ObjectStorageServices(this IServiceCollection services, IConfiguration configuration)
    {
        var objectSection = configuration.GetSection(ObjectStorageSettings.ObjectStorageSettingsConfiguration);
        services.Configure<ObjectStorageSettings>(objectSection);

        services.AddTransient<IObjectStorageService, AmazonS3StorageService>();
    }
}