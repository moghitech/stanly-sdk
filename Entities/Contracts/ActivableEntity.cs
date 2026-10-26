using Fusion.Entities.Auditable;

namespace Fusion.Entities.Contracts;

/// <summary>
/// Represents an entity that can be activated or deactivated.
/// </summary>
public abstract class ActivateableEntity<TKey> : BaseEntity<TKey>, IActivateableEntity<TKey>
{
    /// <summary>
    /// Gets or sets a value indicating whether the entity is active or not.
    /// </summary>
    /// <remarks>
    /// This property determines the active status of the entity. An active entity is considered
    /// operational, while an inactive entity is not. It can be used for enabling or disabling
    /// functionality related to the entity without deleting it.
    /// </remarks>
    public bool IsActive { get; set; }
}
