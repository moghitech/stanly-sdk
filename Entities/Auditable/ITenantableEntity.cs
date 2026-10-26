namespace Fusion.Entities.Auditable;

/// <summary>
/// The abstraction for forcing entities that they need to be Multi-Tenant to have TENANT ID. 
/// </summary>
public interface ITenantableEntity<TKey>
{
    /// <summary>
    /// The ID of tenant referenced for entity.
    /// </summary>
    public TKey TenantId { get; }
}