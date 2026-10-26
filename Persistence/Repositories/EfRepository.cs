using System.Linq.Expressions;
using Fusion.Entities.Contracts;
using Fusion.Persistence.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Fusion.Persistence.Repositories;

/// <inheritdoc cref="IRepository{TEntity,TPrimaryKey}" />
public class EfRepository<TDbContext, TEntity, TPrimaryKey>(TDbContext dbContext)
    : IRepository<TEntity, TPrimaryKey>
    where TEntity : BaseEntity<TPrimaryKey> where TDbContext : DbContext
{
    private readonly DbSet<TEntity> _dbSet = dbContext.Set<TEntity>();

    /// <inheritdoc cref="IRepository{TEntity,TPrimaryKey}.SaveChangesAsync"/>
    public async Task<int> SaveChangesAsync(CancellationToken ct) =>
        await dbContext.SaveChangesAsync(ct);

    /// <summary>
    /// Asynchronously adds a new entity to the database.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="ct"></param>
    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken ct)
    {
        var entry = await _dbSet.AddAsync(entity, ct);

        return entry.Entity;
    }

    /// <summary>
    /// Asynchronously adds a list of entities to the database.
    /// </summary>
    /// <param name="entities">The list of entities to add.</param>
    /// <param name="ct"></param>
    public async Task AddRangeAsync(List<TEntity> entities, CancellationToken ct)
    {
        await _dbSet.AddRangeAsync(entities, ct);
    }

    /// <summary>
    /// Asynchronously performs a hard delete on an entity by ID.
    /// </summary>
    /// <param name="id">The ID of the entity to delete.</param>
    /// <param name="ct"></param>
    public async Task HardDeleteAsync(TPrimaryKey id, CancellationToken ct)
    {
        var link = await _dbSet.FindAsync([id], ct);

        if (link is not null)
            _dbSet.Remove(link);
    }

    /// <summary>
    /// Asynchronously performs a hard delete on an entity by condition.
    /// </summary>
    /// <param name="expression">The expression to delete entities that it satisfied.</param>
    /// <param name="ct"></param>
    public async Task HardDeleteAsync(Expression<Func<TEntity, bool>> expression, CancellationToken ct)
    {
        var entity = _dbSet.Where(expression);

        await entity.ExecuteDeleteAsync(ct);
    }

    /// <inheritdoc cref="IRepository{TEntity,TPrimaryKey}.SoftDeleteAsync(TPrimaryKey,System.Threading.CancellationToken)"/>
    public async Task SoftDeleteAsync(TPrimaryKey id, CancellationToken ct)
    {
        var entity = await _dbSet.FindAsync([id], ct);
        if (entity is AuditableBaseEntity<TPrimaryKey> entityAsSoftDelete)
        {
            entityAsSoftDelete.IsDeleted = true;
            entityAsSoftDelete.RemovedAt = DateTime.UtcNow;
            entityAsSoftDelete.UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <inheritdoc cref="IRepository{TEntity,TPrimaryKey}.SoftDeleteAsync(TPrimaryKey,TPrimaryKey,System.Threading.CancellationToken)"/>
    public async Task SoftDeleteAsync(TPrimaryKey id, TPrimaryKey removerUserId, CancellationToken ct)
    {
        var entity = await _dbSet.FindAsync([id], ct);
        if (entity is AuditableBaseEntity<TPrimaryKey> entityAsSoftDelete)
        {
            entityAsSoftDelete.IsDeleted = true;
            entityAsSoftDelete.RemovedAt = DateTime.UtcNow;
            entityAsSoftDelete.RemoverUserId = removerUserId;
            entityAsSoftDelete.UpdatedAt = DateTime.UtcNow;
            entityAsSoftDelete.UpdaterUserId = removerUserId;
        }
    }

    /// <inheritdoc cref="IRepository{TEntity,TPrimaryKey}.UpdateAsync(TPrimaryKey,TEntity,System.Threading.CancellationToken)"/>
    public async Task<TEntity> UpdateAsync(TPrimaryKey id, TEntity entity, CancellationToken ct)
    {
        var existingEntity = await _dbSet.FindAsync([id], ct);

        if (existingEntity == null)
            throw new InvalidOperationException("Entity not found");

        dbContext.Entry(existingEntity).CurrentValues.SetValues(entity);

        return existingEntity;
    }

    /// <inheritdoc cref="IRepository{TEntity,TPrimaryKey}.UpdateAsync(TPrimaryKey,TEntity,TPrimaryKey,System.Threading.CancellationToken)"/>
    public async Task UpdateAsync(TPrimaryKey id, TEntity entity, TPrimaryKey? updaterUserId, CancellationToken ct)
    {
        var existingEntity = await _dbSet.FindAsync([id], ct);

        if (existingEntity is null) throw new InvalidOperationException("Entity not found");

        dbContext.Entry(existingEntity).CurrentValues.SetValues(entity);

        if (existingEntity is AuditableBaseEntity<TPrimaryKey> auditableEntity)
        {
            auditableEntity.UpdatedAt = DateTime.UtcNow;
            if (updaterUserId is not 0L and not null) auditableEntity.UpdaterUserId = updaterUserId;
        }
    }
}