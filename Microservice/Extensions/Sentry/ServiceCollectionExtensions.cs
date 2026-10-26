using Microsoft.Extensions.DependencyInjection;
using SentryOptions = Fusion.Microservice.Extensions.Options.SentryOptions;

namespace Fusion.Microservice.Extensions.Sentry;

internal static class ServiceCollectionExtensions
{
    public static void AddCustomSentry(this IServiceCollection services, SentryOptions option)
    {
        if(!option.IsEnabled)
            return;

        services.AddSentry();
    }
}