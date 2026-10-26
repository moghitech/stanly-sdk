using System.Linq.Expressions;
using Fusion.Entities;

namespace Fusion.Persistence.Repositories.Contracts;

/// <summary>
/// Defines basic CRUD operations and data access methods for entities, supporting
/// retrieval, insertion, updating, deletion, and pagination of entities.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TPrimaryKey">The type of the primary key for the entity.</typeparam>
public interface IRepository<TEntity, in TPrimaryKey> where TEntity : IEntity<TPrimaryKey>
{
    /// <summary>
    /// Saves and track all entities which changed their states.
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<int> SaveChangesAsync(CancellationToken ct);

    /// <summary>
    /// Asynchronously adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="ct"></param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<TEntity> AddAsync(TEntity entity, CancellationToken ct);

    /// <summary>
    /// Asynchronously adds a range of entities to the repository.
    /// </summary>
    /// <param name="entities">The list of entities to add.</param>
    /// <param name="ct"></param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddRangeAsync(List<TEntity> entities, CancellationToken ct);

    /// <summary>
    /// Updates an existing entity in the data store with the given values.
    /// </summary>
    /// <param name="id">The primary key of the entity to be updated.</param>
    /// <param name="entity">The updated entity values to be applied.</param>
    /// <param name="ct">A token to observe while waiting for the task to complete.</param>
    /// <returns>Returns the updated entity.</returns>
    Task<TEntity> UpdateAsync(TPrimaryKey id, TEntity entity, CancellationToken ct);

    /// <summary>
    /// Updates an existing entity with the specified identifier and replaces its values with the provided entity values.
    /// </summary>
    /// <param name="id">The identifier of the entity to be updated.</param>
    /// <param name="entity">The entity containing the updated values.</param>
    /// <param name="ct">A token to monitor for cancellation requests.</param>
    /// <param name="updaterUserId">An optional identifier of the user performing the update.</param>
    /// <returns>A task.</returns>
    Task UpdateAsync(TPrimaryKey id, TEntity entity, TPrimaryKey? updaterUserId, CancellationToken ct);

    /// <summary>
    /// Asynchronously performs a soft delete of an entity by its primary key.
    /// </summary>
    /// <param name="id">The primary key of the entity to delete.</param>
    /// <param name="ct"></param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SoftDeleteAsync(TPrimaryKey id, CancellationToken ct);

    /// <summary>
    /// Marks an entity as deleted without permanently removing it from the underlying storage.
    /// </summary>
    /// <param name="id">The primary key of the entity to be soft deleted.</param>
    /// <param name="removerUserId">The identifier of the user performing the deletion.</param>
    /// <param name="ct">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SoftDeleteAsync(TPrimaryKey id, TPrimaryKey removerUserId, CancellationToken ct);

    /// <summary>
    /// Asynchronously performs a hard delete of an entity by its primary key.
    /// </summary>
    /// <param name="id">The primary key of the entity to delete.</param>
    /// <param name="ct"></param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task HardDeleteAsync(TPrimaryKey id, CancellationToken ct);

    /// <summary>
    /// Asynchronously performs a hard delete of an entity by provided expression.
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task HardDeleteAsync(Expression<Func<TEntity, bool>> expression, CancellationToken ct);
}