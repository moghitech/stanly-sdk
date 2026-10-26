using Fusion.Microservice.Extensions.Options;
using Microsoft.Extensions.Configuration;

namespace Fusion.Microservice.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IConfiguration"/> to retrieve
/// strongly typed configuration objects used in the application.
/// </summary>
public static class ConfigurationExtensions
{
   internal static ServiceOptions GetServiceOptions(this IConfiguration configuration)
    {
        var serviceOptions = new ServiceOptions();
        configuration.GetSection("ServiceOptions").Bind(serviceOptions);
        return serviceOptions;
    }

    /// <summary>
    /// Retrieves the PostgreSQL database connection options from the
    /// configuration. Depending on the specified parameter, it returns either
    /// readonly or read-write NpgOptions.
    /// </summary>
    /// <param name="configuration">
    /// An instance of <see cref="IConfiguration"/> representing the application configuration.
    /// </param>
    /// <param name="getReadonly">
    /// A boolean value that determines whether to fetch readonly options.
    /// If true, returns readonly NpgOptions; otherwise, returns read-write NpgOptions.
    /// </param>
    /// <returns>
    /// An instance of <see cref="NpgOptions"/> representing the PostgreSQL connection options
    /// with properties configured based on the application configuration.
    /// </returns>
    public static NpgOptions GetNpgOptions(this IConfiguration configuration, bool getReadonly = false)
    {
        var npgOptions = new NpgOptions();

        if (getReadonly) configuration.GetSection("ServiceOptions:NpgReadonlyOptions").Bind(npgOptions);
        else configuration.GetSection("ServiceOptions:NpgOptions").Bind(npgOptions);

        return npgOptions;
    }
}