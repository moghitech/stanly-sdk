namespace Fusion.Messaging.Kafka.Events;

/// <summary>
/// Represents the type of database operation in a Debezium CDC event.
/// </summary>
public enum CdcOperation : byte
{
    /// <summary>
    /// Create/Insert operation - a new record was inserted.
    /// </summary>
    Create = 1,

    /// <summary>
    /// Update operation - an existing record was modified.
    /// </summary>
    Update = 2,

    /// <summary>
    /// Delete operation - a record was removed.
    /// </summary>
    Delete = 3,

    /// <summary>
    /// Read operation - a record was read.
    /// </summary>
    Read = 4,

    /// <summary>
    /// Truncate operation - a table was truncated.
    /// </summary>
    Truncate = 5,

    /// <summary>
    /// Represents an unknown or unsupported operation, typically used when the operation is not recognized
    /// or does not map to a known Debezium CDC operation type.
    /// </summary>
    Unknown = 15
}