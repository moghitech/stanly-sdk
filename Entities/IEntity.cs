namespace Fusion.Entities;

/// <summary>
/// All entities base interface
/// </summary>
public interface IEntity { }

/// <summary>
/// All entities base interface
/// </summary>
/// <typeparam name="TKey">Type of primary key</typeparam>
public interface IEntity<TKey> : IEntity
{
    /// <summary>
    /// primary key
    /// </summary>
    TKey Id { get; set; }
}
