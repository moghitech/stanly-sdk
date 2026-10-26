using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Fusion.Microservice.Extensions.Authorization;

internal static class ClaimTypeMapHelpers
{
    private static readonly Dictionary<string, string> ClaimTypeMapDictionary = new()
    {
        { JwtRegisteredClaimNames.Sub, "sub" },
        { ClaimTypes.NameIdentifier, "nameid" },
        { ClaimTypes.Role, "role" },
        { ClaimTypes.GivenName, "given_name" },
        { ClaimTypes.Surname, "family_name" },
        { ClaimTypes.MobilePhone, ClaimTypes.MobilePhone }
    };

    public static string GetClaimTypeMap(string claimType)
    {
        if (ClaimTypeMapDictionary.TryGetValue(claimType, out var claimTypeMap) &&
            !string.IsNullOrWhiteSpace(claimTypeMap))
            return claimTypeMap;

        return claimType;
    }
}