using System.Net;
using Microsoft.AspNetCore.Http;

namespace Fusion.Extensions;

/// <summary>
/// Provides extension methods for HttpContext to enhance its functionality.
/// </summary>
public static class HttpContextExtensions
{
    /// <summary>
    /// Retrieves the IP address of the client making the request.
    /// </summary>
    /// <param name="httpContext">The current HTTP context containing the request and connection information.</param>
    /// <returns>The IP address of the client as a string. Returns an empty string if the IP address cannot be determined.</returns>
    public static string GetClientIpAddress(this HttpContext httpContext)
    {
        var xForwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(xForwardedFor))
        {
            var firstIp = xForwardedFor.Split(',').FirstOrDefault()?.Trim();
            if (IsValidIpAddress(firstIp))
                return firstIp ?? string.Empty;
        }

        var xRealIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(xRealIp) && IsValidIpAddress(xRealIp))
            return xRealIp;

        var remoteIpAddress = httpContext.Connection.RemoteIpAddress;
        if (remoteIpAddress is null)
            return string.Empty;

        if (remoteIpAddress.IsIPv4MappedToIPv6)
            remoteIpAddress = remoteIpAddress.MapToIPv4();

        return remoteIpAddress.ToString();
    }

    private static bool IsValidIpAddress(string? ipAddress)
    {
        return !string.IsNullOrWhiteSpace(ipAddress) &&
               IPAddress.TryParse(ipAddress, out _);
    }
}