namespace Fusion.Entities.Auditable;

/// <summary>
/// Base for entities with audits
/// </summary>
/// <typeparam name="TKey">Type of primary key</typeparam>
public interface IAuditableEntity<TKey>
    : ICreatableEntity<TKey>,
        IUpdatableEntity<TKey>,
        IRemovableEntity<TKey> { }
