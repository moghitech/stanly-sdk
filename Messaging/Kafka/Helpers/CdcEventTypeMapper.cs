using Fusion.Messaging.Kafka.Events;

namespace Fusion.Messaging.Kafka.Helpers;

/// <summary>
/// Provides utilities for reflecting on Change Data Capture (CDC) event types to retrieve information
/// about legacy entity types associated with the event.
/// </summary>
public static class CdcEventTypeMapper
{
    /// <summary>
    /// Retrieves the legacy entity type associated with the specified event type.
    /// </summary>
    /// <param name="eventType">The event type to be analyzed. This should be a type that inherits from CdcEvent.</param>
    /// <returns>The legacy entity type if found; otherwise, null.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="eventType"/> parameter is null.</exception>
    private static Type? GetLegacyEntityType(Type eventType)
    {
        ArgumentNullException.ThrowIfNull(eventType);

        for (var t = eventType; t is not null && t != typeof(object); t = t.BaseType)
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(CdcEvent<>))
                return t.GetGenericArguments()[0];
        }

        return null;
    }

    /// <summary>
    /// Retrieves the name of the legacy entity type associated with the specified event type.
    /// </summary>
    /// <param name="eventType">The event type to be analyzed. This should be a type that inherits from CdcEvent.</param>
    /// <returns>The name of the legacy entity type if found; otherwise, null.</returns>
    public static string? GetLegacyEntityName(Type eventType)
        => GetLegacyEntityType(eventType)?.Name;

    /// <summary>
    /// Retrieves the full name of the legacy entity type associated with the specified event type.
    /// </summary>
    /// <param name="eventType">The event type to be analyzed. This should be a type that inherits from CdcEvent;</param>
    /// <returns>The full name of the legacy entity type if found; otherwise, null.</returns>
    public static string? GetLegacyEntityFullName(Type eventType)
        => GetLegacyEntityType(eventType)?.FullName;
}