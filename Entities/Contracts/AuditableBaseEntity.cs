using System.ComponentModel.DataAnnotations;
using Fusion.Entities.Auditable;

namespace Fusion.Entities.Contracts;

/// <summary>
/// Base for entities with audits
/// </summary>
/// <typeparam name="TKey">Type of primary key</typeparam>
public abstract class AuditableBaseEntity<TKey> : BaseEntity<TKey>, IAuditableEntity<TKey>
{
    /// <summary>
    /// Time of row creation
    /// </summary>
    [Required]
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// ID of creator user
    /// </summary>
    [Required]
    public TKey CreatorUserId { get; set; } = default!;

    /// <summary>
    /// Time of row modifier
    /// </summary>
    [Required]
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// ID of modifier user
    /// </summary>
    [Required]
    public TKey UpdaterUserId { get; set; } = default!;

    /// <summary>
    /// Row is removed?
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Time of row removal
    /// </summary>
    public DateTime? RemovedAt { get; set; } = null;

    /// <summary>
    /// ID of remover user
    /// </summary>
    public TKey? RemoverUserId { get; set; }
}