using System.ComponentModel.DataAnnotations;

namespace Fusion.Entities.Auditable;

/// <summary>
/// Base for entities with changes
/// </summary>
/// <typeparam name="TKey">Type of primary key</typeparam>
public interface IUpdatableEntity<TKey>
{
    /// <summary>
    /// Time of row update
    /// </summary>
    [Required]
    DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// ID of updater user
    /// </summary>
    [Required]
    TKey UpdaterUserId { get; set; }
}