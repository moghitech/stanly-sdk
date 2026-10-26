namespace Fusion.Client;

/// <summary>
/// Client URI
/// </summary>
/// <param name="BaseUrl"></param>
/// <param name="Version"></param>
public record ClientUri(
    string BaseUrl,
    string Version,
    string Suffix = "api"
)
{
    internal string CalculateUri()
    {
        var trimmedBaseUrl = BaseUrl.Trim('/').TrimStart('/');
        var trimmedSuffix = Suffix.Trim('/').TrimStart('/');
        var trimmedVersion = Version.Trim('/').TrimStart('/');

        return $"{trimmedBaseUrl}/{trimmedSuffix}/{trimmedVersion}";
    }
}