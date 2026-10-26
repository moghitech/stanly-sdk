namespace Fusion.Microservice.Extensions.Options;

/// <summary>
/// Represents configuration options for defining authorization policies
/// and settings within a given application context.
/// </summary>
public class AuthorizationOptions
{
    private readonly List<string> _roles = new();

    /// <summary>
    /// Gets the list of configured roles.
    /// </summary>
    public IReadOnlyList<string> Roles => _roles.AsReadOnly();

    /// <summary>
    /// Adds a role to the authorization configuration.
    /// </summary>
    /// <param name="role">The role name to add.</param>
    /// <returns>The current AuthorizationOptions instance for method chaining.</returns>
    public AuthorizationOptions AddRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
            throw new ArgumentException("Role name cannot be null or empty.", nameof(role));

        if (!_roles.Contains(role))
            _roles.Add(role);

        return this;
    }
}