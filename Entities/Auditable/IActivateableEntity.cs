namespace Fusion.Entities.Auditable;

/// <summary>
/// Represents an entity with the ability to be activated or deactivated.
/// </summary>
public interface IActivateableEntity<TKey> : IEntity<TKey>
{
    bool IsActive { get; set; }
}