namespace Fusion.Messaging.Kafka.Events;

/// <summary>
/// Represents information about the source of a change data capture (CDC) event,
/// providing metadata such as the originating database, schema, and table.
/// </summary>
public class CdcSourceEvent
{
    /// <summary>
    /// Gets or sets the name associated with the source of the change data capture (CDC) event.
    /// This could be used to identify or label the origin of the event.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the name of the database associated with the source of the change data capture (CDC) event.
    /// This property identifies the database where the event originated.
    /// </summary>
    public string? Db { get; set; }

    /// <summary>
    /// Gets or sets the schema associated with the source of the change data capture (CDC) event.
    /// This represents the database schema that contains the table related to the event.
    /// </summary>
    public string? Schema { get; set; }

    /// <summary>
    /// Gets or sets the name of the table associated with the source of the change data capture (CDC) event.
    /// This property specifies the table within the database where the event originated.
    /// </summary>
    public string? Table { get; set; }
}