namespace Fusion.Microservice.Extensions.AppVersion;

internal static class AppVersionAccessor
{
    public static string GetAppVersion()
    {
        return $"1.0.{GetPipelineId()}";
    }

    public static string GetGitCommitSha()
    {
        return Environment.GetEnvironmentVariable(AppVersionConstants.GitCommitSha) ?? "not specified";
    }

    public static string GetPipelineId()
    {
        return Environment.GetEnvironmentVariable(AppVersionConstants.PipeLineId) ?? "not specified";
    }
}