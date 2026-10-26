using System.ComponentModel.DataAnnotations;

namespace Fusion.Entities.Auditable;

/// <summary>
/// Base for entities with soft delete
/// </summary>
/// <typeparam name="TKey">Type of primary key</typeparam>
public interface IRemovableEntity<TKey>
{
    /// <summary>
    /// Row is removed?
    /// </summary>
    [Required]
    bool IsDeleted { get; set; }

    /// <summary>
    /// Time of row removal
    /// </summary>
    [Required]
    DateTime? RemovedAt { get; set; }

    /// <summary>
    /// ID of remover user
    /// </summary>
    TKey? RemoverUserId { get; set; }
}
