using System.Text.RegularExpressions;

namespace Fusion.Messaging.Kafka.Helpers;

/// <summary>
/// Provides functionality for constructing case-insensitive topic wildcards
/// designed to match specific patterns in Kafka topics used for change data capture (CDC).
/// </summary>
public static class CdcTopicWildcardBuilder
{
    /// <summary>
    /// Constructs a case-insensitive wildcard pattern for a Kafka topic
    /// that matches the specified entity name.
    /// </summary>
    /// <param name="entityName">The name of the entity to include in the wildcard pattern.
    /// This value must not be null, empty, or contain only whitespace.</param>
    /// <returns>A string representing the wildcard pattern
    /// that matches Kafka topics associated with the given entity name.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="entityName"/> is null, empty, or contains only whitespace.</exception>
    public static string ForLegacyEntity(string entityName)
    {
        if (string.IsNullOrWhiteSpace(entityName))
            throw new ArgumentException("Entity is required.", nameof(entityName));

        var escapedEntityName = Regex.Escape(entityName.Trim());

        return $"^.*{escapedEntityName}.*";
    }
}