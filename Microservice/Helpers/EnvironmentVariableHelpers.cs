using Fusion.Exceptions;

namespace Fusion.Microservice.Helpers;

internal static class EnvironmentVariableHelpers
{
    private const string ServiceNameEnvVariable = "NS_SERVICE_NAME";

    internal static void SetServiceName(string serviceName)
    {
        Environment.SetEnvironmentVariable(ServiceNameEnvVariable, serviceName);
    }

    internal static string GetServiceName()
    {
        var serviceName = Environment.GetEnvironmentVariable(ServiceNameEnvVariable);
        if (string.IsNullOrWhiteSpace(serviceName))
            throw new RuntimeException("Service name is not set in environment variables");
        return serviceName;
    }

    internal static string GetEnvironmentName()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown";
    }
}