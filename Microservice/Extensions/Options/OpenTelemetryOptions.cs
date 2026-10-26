namespace Fusion.Microservice.Extensions.Options;

public class OpenTelemetryOptions
{
    public required bool IsEnabled { get; set; }
    public required string Endpoint { get; set; }
}