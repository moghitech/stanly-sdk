using System.ComponentModel.DataAnnotations;
using Fusion.Entities.Auditable;

namespace Fusion.Entities.Contracts;

/// <summary>
/// Base for entities with creation
/// </summary>
/// <typeparam name="TKey">Type of primary key</typeparam>
public abstract class InsertableBaseEntity<TKey> : BaseEntity<TKey>, ICreatableEntity<TKey>
{
    /// <summary>
    /// Time of row creation
    /// </summary>
    [Required]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// ID of creator user
    /// </summary>
    [Required]
    public TKey CreatorUserId { get; set; } = default!;
}