namespace Fusion.Client.DTOs;

/// <summary>
/// Represents the result of the proxy authentication process.
/// </summary>
public record ProxyAuthentication(
    string Token,
    string HeaderName,
    string Prefix
)
{
    /// <summary>
    /// Creates a new instance of <see cref="ProxyAuthentication"/> with the specified token,
    /// pre-configured to use the "Authorization" header and a "Bearer" prefix.
    /// </summary>
    /// <param name="token">The bearer token used for authentication.</param>
    /// <returns>A <see cref="ProxyAuthentication"/> initialized with the provided token and default header and prefix values.</returns>
    public static ProxyAuthentication FromBearer(string token)
    {
        return new ProxyAuthentication(token, "Authorization", "Bearer");
    }
}