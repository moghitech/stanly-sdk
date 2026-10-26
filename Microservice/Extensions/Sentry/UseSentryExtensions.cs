using Fusion.Exceptions;
using Fusion.Exceptions.BusinessExceptions;
using Microsoft.AspNetCore.Hosting;
using SentryOptions = Fusion.Microservice.Extensions.Options.SentryOptions;

namespace Fusion.Microservice.Extensions.Sentry;

internal static class UseSentryExtensions
{
    public static void UseCustomSentry(this Microsoft.AspNetCore.Builder.WebApplicationBuilder builder,
        SentryOptions option)
    {
        if (!option.IsEnabled)
            return;

        builder.WebHost.UseSentry(o =>
        {
            o.Dsn = option.Dsn;
            o.SendDefaultPii = option.SendDefaultPii;
            o.MaxRequestBodySize = option.MaxRequestBodySize;
            o.MinimumBreadcrumbLevel = option.MinimumBreadcrumbLevel;
            o.MinimumEventLevel = option.MinimumEventLevel;
            o.AttachStacktrace = option.AttachStackTrace;
            o.Debug = option.Debug;
            o.DiagnosticLevel = option.DiagnosticLevel;
            o.TracesSampleRate = option.TracesSampleRate;

            o.AddExceptionFilterForType<BusinessException>();
            o.AddExceptionFilterForType<InvalidOperationException>();
            o.AddExceptionFilterForType<BaseHttpException>();
        });
    }
}