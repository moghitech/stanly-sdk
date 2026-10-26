using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Fusion.Microservice.Extensions.Authorization.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Fusion.Microservice.Extensions.Authorization;

internal class JwtRoleOnlyAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authHeader = Request.Headers.Authorization.ToString();
        if (string.IsNullOrWhiteSpace(authHeader))
            return Task.FromResult(AuthenticateResult.Fail("Missing Authorization header"));

        if (!authHeader.StartsWith("Bearer "))
            return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization header format"));

        var token = authHeader["Bearer ".Length..].Trim();

        try
        {
            var claims = ExtractClaimsFromToken(token);
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
        catch (Exception ex)
        {
            return Task.FromResult(AuthenticateResult.Fail($"Token parsing failed: {ex.Message}"));
        }
    }

    private static List<Claim> ExtractClaimsFromToken(string token)
    {
        var claims = new List<Claim>();

        var tokenParts = token.Split('.');
        if (tokenParts.Length != 3)
            throw new ArgumentException("Invalid JWT token format - must have 3 parts separated by dots");

        var payload = tokenParts[1];

        try
        {
            // Proper Base64 URL decoding
            var payloadBytes = DecodeBase64Url(payload);
            var payloadJson = Encoding.UTF8.GetString(payloadBytes);

            using var document = JsonDocument.Parse(payloadJson);
            var root = document.RootElement;

            // Extract claims with proper null checking
            AddClaimIfExists(claims, root, JwtRegisteredClaimNames.Sub, JwtRegisteredClaimNames.Sub);
            AddClaimIfExists(claims, root, ClaimTypes.NameIdentifier, ClaimTypes.NameIdentifier);
            AddClaimIfExists(claims, root, ClaimTypes.GivenName, ClaimTypes.GivenName);
            AddClaimIfExists(claims, root, ClaimTypes.Surname, ClaimTypes.Surname);
            AddClaimIfExists(claims, root, ClaimTypes.MobilePhone, ClaimTypes.MobilePhone);
            AddClaimIfExists(claims, root, CustomClaimsConstants.UserType, CustomClaimsConstants.UserType);
            AddClaimIfExists(claims, root, CustomClaimsConstants.CustomerId, CustomClaimsConstants.CustomerId);
            AddClaimIfExists(claims, root, CustomClaimsConstants.LegacyUserId, CustomClaimsConstants.LegacyUserId);
            AddClaimIfExists(claims, root, CustomClaimsConstants.LegacyBranchId, CustomClaimsConstants.LegacyBranchId);
            AddClaimIfExists(claims, root, CustomClaimsConstants.ApplicationUserType, CustomClaimsConstants.ApplicationUserType);

            // Handle roles (can be string or array)
            ExtractRoleClaims(claims, root);

            return claims;
        }
        catch (JsonException ex)
        {
            throw new ArgumentException($"Invalid JSON in JWT payload: {ex.Message}", ex);
        }
        catch (FormatException ex)
        {
            throw new ArgumentException($"Invalid Base64 encoding in JWT payload: {ex.Message}", ex);
        }
    }

    private static byte[] DecodeBase64Url(string base64Url)
    {
        if (string.IsNullOrEmpty(base64Url))
            throw new ArgumentException("Base64 string cannot be null or empty");

        // Replace URL-safe characters
        var base64 = base64Url.Replace('-', '+').Replace('_', '/');

        // Add padding if needed
        var padding = base64.Length % 4;
        if (padding > 0)
            base64 += new string('=', 4 - padding);

        try
        {
            return Convert.FromBase64String(base64);
        }
        catch (FormatException ex)
        {
            throw new FormatException($"Invalid Base64 string: {base64}", ex);
        }
    }

    private static void AddClaimIfExists(List<Claim> claims, JsonElement root, string jsonPropertyName,
        string claimType)
    {
        var mappedPropertyName = ClaimTypeMapHelpers.GetClaimTypeMap(jsonPropertyName);

        if (!root.TryGetProperty(mappedPropertyName, out var element) || element.ValueKind != JsonValueKind.String)
            return;

        var value = element.GetString();
        if (!string.IsNullOrEmpty(value))
        {
            claims.Add(new Claim(claimType, value));
        }
    }

    private static void ExtractRoleClaims(List<Claim> claims, JsonElement root)
    {
        var roleMappedName = ClaimTypeMapHelpers.GetClaimTypeMap(ClaimTypes.Role);

        if (!root.TryGetProperty(roleMappedName, out var roleElement))
            return;

        switch (roleElement.ValueKind)
        {
            case JsonValueKind.Array:
                foreach (var role in roleElement.EnumerateArray())
                {
                    if (role.ValueKind != JsonValueKind.String)
                        continue;
                    var roleValue = role.GetString();
                    if (!string.IsNullOrEmpty(roleValue))
                    {
                        claims.Add(new Claim(ClaimTypes.Role, roleValue));
                    }
                }

                break;

            case JsonValueKind.String:
                var singleRoleValue = roleElement.GetString();
                if (!string.IsNullOrEmpty(singleRoleValue))
                {
                    claims.Add(new Claim(ClaimTypes.Role, singleRoleValue));
                }

                break;
        }
    }
}