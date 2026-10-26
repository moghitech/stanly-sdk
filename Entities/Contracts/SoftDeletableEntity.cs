namespace Fusion.Entities.Contracts;

/// <summary>
/// Represents an entity that supports soft deletion functionality.
/// Inherits from the <see cref="BaseEntity{T}"/> class.
/// </summary>
/// <typeparam name="T">The type of the primary key.</typeparam>
public class SoftDeletableEntity<T> : BaseEntity<T>
{
    /// <summary>
    /// Gets or sets the timestamp indicating when the entity was soft deleted.
    /// </summary>
    /// <remarks>
    /// If the value is null, the entity has not been soft deleted.
    /// A non-null value represents the date and time the entity was marked as deleted.
    /// </remarks>
    public DateTimeOffset? DeletedAt { get; set; }
}
