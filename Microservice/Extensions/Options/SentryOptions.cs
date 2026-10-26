using Microsoft.Extensions.Logging;
using Sentry.Extensibility;

namespace Fusion.Microservice.Extensions.Options;

public class SentryOptions
{
    public bool IsEnabled { get; set; }
    public string Dsn { get; set; } = string.Empty;
    public bool SendDefaultPii { get; set; }
    public RequestSize MaxRequestBodySize { get; set; } = RequestSize.Always;
    public LogLevel MinimumBreadcrumbLevel { get; set; } = LogLevel.Information;
    public LogLevel MinimumEventLevel { get; set; } = LogLevel.Error;
    public bool AttachStackTrace { get; set; }
    public bool Debug { get; set; }
    public SentryLevel DiagnosticLevel { get; set; } = SentryLevel.Error;
    public double TracesSampleRate { get; set; }
}