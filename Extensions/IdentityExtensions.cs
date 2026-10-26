using System.Globalization;
using System.Security.Claims;
using System.Security.Principal;
using Fusion.Microservice.Extensions.Authorization.Constants;

namespace Fusion.Extensions;

/// <summary>
/// Provides extension methods for working with <see cref="IIdentity"/> and <see cref="ClaimsIdentity"/>
/// to retrieve specific claim or user information.
/// </summary>
public static class IdentityExtensions
{
    /// <summary>
    /// Retrieves the user's identifier from the identity object and converts it to the specified type.
    /// </summary>
    /// <typeparam name="T">
    /// The type to which the user identifier should be converted.
    /// Must implement <see cref="System.IConvertible"/>.
    /// </typeparam>
    /// <param name="identity">
    /// The identity object that contains the user's claim information.
    /// </param>
    /// <returns>
    /// The user identifier as a value of type <typeparamref name="T"/>
    /// if the identifier exists and is successfully converted; otherwise, returns the default value of <typeparamref name="T"/>.
    /// </returns>
    public static T GetUserId<T>(this IIdentity identity) where T : IConvertible
    {
        var userId = identity.GetUserId();
        return (string.IsNullOrEmpty(userId)
            ? default
            : (T)Convert.ChangeType(userId, typeof(T), CultureInfo.InvariantCulture))!;
    }

    /// <summary>
    /// Retrieves the user's identifier from the identity object, ensuring that it exists and is not empty,
    /// and converts it to the specified type. Throws an exception if the identifier is missing or null.
    /// </summary>
    /// <typeparam name="T">
    /// The type to which the user identifier should be converted.
    /// Must implement <see cref="System.IConvertible"/>.
    /// </typeparam>
    /// <param name="identity">
    /// The identity object that contains the user's claim information.
    /// </param>
    /// <returns>
    /// The user identifier as a value of type <typeparamref name="T"/> if it exists and is successfully converted.
    /// </returns>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown when the identity object is null or the user identifier is missing or empty.
    /// </exception>
    public static T GetUserIdRequired<T>(this IIdentity? identity) where T : IConvertible
    {
        if (identity is null)
            throw new UnauthorizedAccessException();

        var userId = identity.GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
            throw new UnauthorizedAccessException();

        return (string.IsNullOrEmpty(userId)
            ? default
            : (T)Convert.ChangeType(userId, typeof(T), CultureInfo.InvariantCulture))!;
    }

    /// <summary>
    /// Retrieves the user ID from the provided <see cref="IIdentity"/> instance as a long value.
    /// Throws an <see cref="UnauthorizedAccessException"/> if the user ID is not found or is not valid.
    /// </summary>
    /// <param name="identity">The <see cref="IIdentity"/> from which the user ID is retrieved.</param>
    /// <returns>The user ID as a long.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown if the user ID is null, empty, or not a valid long.</exception>
    public static long GetUserIdRequired(this IIdentity? identity)
    {
        if (identity is null)
            throw new UnauthorizedAccessException();

        var strUserId = identity.GetUserId();

        return long.TryParse(strUserId, out var userId)
            ? userId
            : throw new UnauthorizedAccessException();
    }

    /// <summary>
    /// Retrieves the user's identifier from the identity object.
    /// </summary>
    /// <param name="identity">
    /// The identity object that contains the user's claim information.
    /// Must implement <see cref="IIdentity"/>.
    /// </param>
    /// <returns>
    /// The user identifier as a string if the identifier exists;
    /// otherwise, returns null or an empty string.
    /// </returns>
    public static string GetUserId(this IIdentity identity)
    {
        return identity.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    /// <summary>
    /// Retrieves the email address of the user from the identity object.
    /// </summary>
    /// <param name="identity">
    /// The identity object that contains the user's claim information.
    /// </param>
    /// <returns>
    /// The user's email address if it exists; otherwise, returns an empty string.
    /// </returns>
    public static string GetEmail(this IIdentity identity)
    {
        return identity.FindFirstValue(ClaimTypes.Email);
    }

    /// <summary>
    /// Retrieves the username of the user from the identity object by extracting the claim value for the name.
    /// </summary>
    /// <param name="identity">
    /// The identity object that contains the user's claim information.
    /// </param>
    /// <returns>
    /// The username of the user if the claim exists; otherwise, returns an empty string.
    /// </returns>
    public static string GetUsername(this IIdentity identity)
    {
        return identity.FindFirstValue(ClaimTypes.Name);
    }

    /// <summary>
    /// Retrieves the value of the first claim that matches the specified claim type from the claims identity.
    /// </summary>
    /// <param name="identity">
    /// The identity object that represents the claims-based identity.
    /// Must be of type <see cref="ClaimsIdentity"/>.
    /// </param>
    /// <param name="claimType">
    /// The type of the claim to retrieve.
    /// </param>
    /// <returns>
    /// The value of the first claim matching the specified type, or an empty string if no such claim exists.
    /// </returns>
    public static string FindFirstValue(this ClaimsIdentity identity, string claimType)
    {
        return identity.FindFirst(claimType)?.Value ?? string.Empty;
    }

    /// <summary>
    /// Retrieves the value of the first claim of the specified claim type from the identity.
    /// </summary>
    /// <param name="identity">
    /// The identity object that represents the user. Must implement <see cref="IIdentity"/>.
    /// </param>
    /// <param name="claimType">
    /// The type of the claim to retrieve.
    /// </param>
    /// <returns>
    /// The value of the first claim matching the specified type, or an empty string if no such claim exists or the identity is not of type <see cref="ClaimsIdentity"/>.
    /// </returns>
    public static string FindFirstValue(this IIdentity identity, string claimType)
    {
        var claimsIdentity = identity as ClaimsIdentity;
        return claimsIdentity?.FindFirstValue(claimType) ?? string.Empty;
    }

    /// <summary>
    /// Retrieves the customer identifier from the specified identity object as a required value.
    /// Throws an <see cref="UnauthorizedAccessException"/> if the identity is null or the customer identifier is not available.
    /// </summary>
    /// <param name="identity">
    /// The identity object that contains the claims, from which the customer identifier will be retrieved.
    /// </param>
    /// <returns>
    /// The customer identifier as a <see cref="long"/>.
    /// </returns>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown when the identity is null, the customer identifier claim is not found,
    /// or it contains a null or empty value.
    /// </exception>
    public static long GetCustomerIdRequired(this IIdentity? identity)
    {
        if (identity is null)
            throw new UnauthorizedAccessException();

        var customerId = identity.FindFirstValue(CustomClaimsConstants.CustomerId);
        if (string.IsNullOrWhiteSpace(customerId))
            throw new UnauthorizedAccessException();

        return string.IsNullOrEmpty(customerId)
            ? 0
            : Convert.ToInt64(customerId, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Retrieves the legacy user identifier from the identity object.
    /// Throws an <see cref="UnauthorizedAccessException"/> if the identity is null
    /// or if the legacy user identifier is missing or invalid.
    /// </summary>
    /// <param name="identity">
    /// The identity object that contains the user's claims.
    /// Can be null, in which case an exception is thrown.
    /// </param>
    /// <returns>
    /// The legacy user identifier as a <see cref="long"/> if it exists and is valid;
    /// otherwise, throws an exception.
    /// </returns>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown if the identity is null or if the legacy user identifier claim is missing or invalid.
    /// </exception>
    public static long GetLegacyUserIdRequired(this IIdentity? identity)
    {
        if (identity is null)
            throw new UnauthorizedAccessException();

        var legacyUserId = identity.FindFirstValue(CustomClaimsConstants.LegacyUserId);
        if (string.IsNullOrWhiteSpace(legacyUserId))
            throw new UnauthorizedAccessException();

        return string.IsNullOrEmpty(legacyUserId)
            ? 0
            : Convert.ToInt64(legacyUserId, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Retrieves the legacy branch identifier associated with the specified identity.
    /// Throws an exception if the identity is null or the legacy branch identifier claim is missing or invalid.
    /// </summary>
    /// <param name="identity">
    /// The identity object containing claim information, which may include the legacy branch identifier.
    /// </param>
    /// <returns>
    /// The legacy branch identifier as a long integer if it exists and is valid.
    /// </returns>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown when the identity is null or the legacy branch identifier is missing or invalid.
    /// </exception>
    public static long GetLegacyBranchIdRequired(this IIdentity? identity)
    {
        if (identity is null)
            throw new UnauthorizedAccessException();

        var legacyUserId = identity.FindFirstValue(CustomClaimsConstants.LegacyBranchId);
        if (string.IsNullOrWhiteSpace(legacyUserId))
            throw new UnauthorizedAccessException();

        return string.IsNullOrEmpty(legacyUserId)
            ? 0
            : Convert.ToInt64(legacyUserId, CultureInfo.InvariantCulture);
    }
}