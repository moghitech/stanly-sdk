using System.ComponentModel.DataAnnotations;

namespace Fusion.Entities.Auditable;

/// <summary>
/// Represents a read-only model entity with audit information.
/// This interface ensures that the entity tracks the creation timestamp and the identifier of the user who created it.
/// </summary>
/// <typeparam name="TKey">The type of the identifier for the creator user.</typeparam>
public interface IReadModelEntity<TKey>
{
    /// <summary>
    /// Time of row update
    /// </summary>
    [Required]
    DateTimeOffset UpdatedAt { get; set; }
}