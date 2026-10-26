namespace Fusion.Microservice.Extensions.Authorization.Constants;

/// <summary>
/// Provides a set of constants representing custom claim types used in the authorization implementation.
/// </summary>
public static class CustomClaimsConstants
{
    /// <summary>
    /// Defines a specific claim type used to uniquely identify the security stamp associated with a user.
    /// This identifier is essential for tracking modifications to security-critical user data, including password
    /// resets or adjustments to authentication configurations.
    /// </summary>
    public const string SecurityStamp = "security_stamp";

    /// <summary>
    /// Represents a custom claim type indicating the type of user within the authorization system.
    /// This constant value is stored in JWT tokens and can be used to identify or differentiate
    /// between various user types for access control and authorization purposes.
    /// </summary>
    public const string UserType = "user_type";

    /// <summary>
    /// Represents a custom claim type used to enforce validation of the security stamp associated with a user.
    /// This constant is utilized to ensure that any critical security updates, such as password changes,
    /// result in the invalidation of existing authentication tokens.
    /// </summary>
    public const string EnforceSecurityStampValidation = "enforce_security_stamp";

    /// <summary>
    /// Represents a custom claim type used to uniquely identify the customer associated with a user.
    /// This constant is leveraged in authorization mechanisms to associate specific data or access
    /// permissions to a particular customer identifier.
    /// </summary>
    public const string CustomerId = "customer_id";

    /// <summary>
    /// Defines a specific claim type used to categorize or identify the type of application user.
    /// This claim is crucial for distinguishing various user roles or types to facilitate role-based
    /// access control and ensure appropriate authorization within the application.
    /// </summary>
    public const string ApplicationUserType = "application_user_type";

    /// <summary>
    /// Represents a custom claim type used to reference the legacy system's unique user identifier.
    /// This claim is primarily utilized for integration or migration purposes, enabling the association
    /// of users across systems that rely on different identification schemas.
    /// </summary>
    public const string LegacyUserId = "legacy_user_id";

    /// <summary>
    /// Represents a custom claim type used to identify the legacy branch information
    /// associated with a user or entity. This claim is utilized in scenarios where
    /// legacy support or backward compatibility with older systems is required.
    /// </summary>
    public const string LegacyBranchId = "legacy_branch_id";

    /// <summary>
    /// Represents a specific claim type used to uniquely identify the branch associated with a user
    /// or action within the system. This value is typically utilized to enforce branch-specific
    /// authorization rules or to ensure accurate assignment of resources or permissions.
    /// </summary>
    public const string BranchId = "branch_id";
}