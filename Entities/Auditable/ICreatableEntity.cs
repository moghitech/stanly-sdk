using System.ComponentModel.DataAnnotations;

namespace Fusion.Entities.Auditable;

/// <summary>
/// Base for entities with creation
/// </summary>
/// <typeparam name="TKey">Type of primary key</typeparam>
public interface ICreatableEntity<TKey>
{
    /// <summary>
    /// Time of row creation
    /// </summary>
    [Required]
    DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// ID of creator user
    /// </summary>
    [Required]
    TKey CreatorUserId { get; set; }
}
