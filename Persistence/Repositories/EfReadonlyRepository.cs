using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Fusion.Entities.Contracts;
using Fusion.Microservice.Models;
using Fusion.Persistence.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Fusion.Persistence.Repositories;

/// <inheritdoc cref="IRepository{TEntity,TPrimaryKey}" />
public class EfReadonlyRepository<TDbContext, TEntity, TPrimaryKey>(TDbContext dbContext)
    : IReadonlyRepository<TEntity, TPrimaryKey>
    where TEntity : BaseEntity<TPrimaryKey> where TDbContext : DbContext
{
    private const int DefaultPageSize = 25;
    private const int DefaultPageNumber = 1;

    private readonly DbSet<TEntity> _dbSet = dbContext.Set<TEntity>();

    /// <inheritdoc cref="IReadonlyRepository{TEntity,TPrimaryKey}.ExistsAsync"/>
    public async Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0
    )
    {
        var tag = GetQueryTag(callerMemberName, callerFilePath, callerLineNumber);

        var query = _dbSet.AsQueryable()
            .TagWith(tag)
            .Where(predicate);

        return await query.AnyAsync(cancellationToken: cancellationToken);
    }

    /// <inheritdoc cref="IReadonlyRepository{TEntity,TPrimaryKey}.GetAllAsQuery"/>
    public IQueryable<TEntity> GetAllAsQuery(
        List<Expression<Func<TEntity, object>>>? includeExpressions = null,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0
    )
    {
        var tag = GetQueryTag(callerMemberName, callerFilePath, callerLineNumber);

        var query = _dbSet.AsQueryable()
            .TagWith(tag);

        if (includeExpressions is not null && includeExpressions.Count is not 0)
        {
            query = includeExpressions.Aggregate(query,
                (current, includeExpression) => current.Include(includeExpression));
        }

        return query;
    }

    /// <inheritdoc cref="IReadonlyRepository{TEntity,TPrimaryKey}.GetAllAsync"/>
    public async Task<IEnumerable<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>> expression,
        List<Expression<Func<TEntity, object>>>? includeExpressions = null,
        CancellationToken cancellationToken = default,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0
    )
    {
        var tag = GetQueryTag(callerMemberName, callerFilePath, callerLineNumber);

        var query = _dbSet.AsQueryable()
            .TagWith(tag);

        if (includeExpressions is null || includeExpressions.Count is 0)
            return await query.Where(expression).ToListAsync(cancellationToken);

        query = includeExpressions.Aggregate(query, (current, includeExpression)
            => current.Include(includeExpression));

        query = query.Where(expression);

        return await query.ToListAsync(cancellationToken);
    }

    /// <inheritdoc cref="IReadonlyRepository{TEntity,TPrimaryKey}.GetAsNoTracking"/>
    public IQueryable<TEntity> GetAsNoTracking(
        Expression<Func<TEntity, bool>> expression,
        List<Expression<Func<TEntity, object>>>? includeExpressions = null,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0
    )
    {
        var tag = GetQueryTag(callerMemberName, callerFilePath, callerLineNumber);

        var query = _dbSet.AsQueryable()
            .TagWith(tag);

        if (includeExpressions is not null && includeExpressions.Count is not 0)
            query = includeExpressions.Aggregate(query,
                (current, includeExpression) => current.Include(includeExpression));

        query = query.Where(expression);

        return query;
    }

    /// <inheritdoc cref="IReadonlyRepository{TEntity,TPrimaryKey}.GetByIdAsync"/>
    public async Task<TEntity?> GetByIdAsync(TPrimaryKey id,
        List<Expression<Func<TEntity, object>>>? includeExpressions = null,
        CancellationToken cancellationToken = default,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0
    )
    {
        var tag = GetQueryTag(callerMemberName, callerFilePath, callerLineNumber);

        var query = _dbSet
            .TagWith(tag)
            .Where(e => e.Id!.Equals(id));

        if (includeExpressions == null || includeExpressions.Count == 0)
            return await query.FirstOrDefaultAsync(cancellationToken);

        query = includeExpressions.Aggregate(query, (current, includeExpression)
            => current.Include(includeExpression));

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc cref="IReadonlyRepository{TEntity,TPrimaryKey}.GetAllPaginatedAsync(System.Linq.IQueryable{TEntity},int,int,System.Threading.CancellationToken,string,string,int)"/>
    public async Task<List<TEntity>> GetAllPaginatedAsync(
        IQueryable<TEntity> query,
        int take,
        int skip,
        CancellationToken cancellationToken = default,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0
    )
    {
        var tag = GetQueryTag(callerMemberName, callerFilePath, callerLineNumber);

        return await query
            .TagWith(tag)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc cref="IReadonlyRepository{TEntity,TPrimaryKey}.GetAllPaginatedAsync(System.Linq.IQueryable{TEntity},int,int,System.Threading.CancellationToken,string,string,int)"/>
    public async Task<Paginated<TEntity>> GetAllPaginatedAsync(
        int take,
        int skip,
        CancellationToken cancellationToken,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0
    )
    {
        var tag = GetQueryTag(callerMemberName, callerFilePath, callerLineNumber);

        var query = _dbSet.AsQueryable()
            .TagWith(tag);

        var count = await _dbSet.CountAsync(cancellationToken);

        var items = await query
            .Take(take)
            .Skip(skip)
            .ToListAsync(cancellationToken);

        return new Paginated<TEntity>(items, count);
    }

    /// <inheritdoc cref="IReadonlyRepository{TEntity,TPrimaryKey}.GetAllPaginatedAsync(int?,int?,Expression{System.Func{TEntity,bool}},List{Expression{System.Func{TEntity,object}}},System.Threading.CancellationToken,string,string,int)"/>
    public async Task<Paginated<TEntity>> GetAllPaginatedAsync(
        int? pageSize,
        int? pageNumber,
        Expression<Func<TEntity, bool>>? expression,
        List<Expression<Func<TEntity, object>>>? includeExpressions,
        CancellationToken cancellationToken,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0
    )
    {
        var tag = GetQueryTag(callerMemberName, callerFilePath, callerLineNumber);

        pageNumber ??= DefaultPageNumber;
        pageSize ??= DefaultPageSize;

        var query = _dbSet.AsQueryable()
            .TagWith(tag);

        if (includeExpressions is { Count: > 0 })
            query = includeExpressions.Aggregate(query,
                (current, includeExpression) => current.Include(includeExpression));

        if (expression is not null)
            query = query.Where(expression);

        var count = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber.Value - 1) * pageSize.Value)
            .Take(pageSize.Value)
            .ToListAsync(cancellationToken);

        return new Paginated<TEntity>(items, count, pageNumber.Value, pageSize.Value);
    }


    /// <inheritdoc cref="IReadonlyRepository{TEntity,TPrimaryKey}.GetAllPaginatedAsync(System.Linq.IQueryable{TEntity},int,int,System.Threading.CancellationToken,string,string,int)"/>
    public async Task<Paginated<TEntity>> GetAllPaginatedAsync(
        IQueryable<TEntity> query,
        int? pageSize,
        int? pageNumber,
        CancellationToken cancellationToken,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0
    )
    {
        var tag = GetQueryTag(callerMemberName, callerFilePath, callerLineNumber);

        pageNumber ??= DefaultPageNumber;
        pageSize ??= DefaultPageSize;

        var count = await query
            .TagWith(tag)
            .CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber.Value - 1) * pageSize.Value)
            .Take(pageSize.Value)
            .ToListAsync(cancellationToken);

        return new Paginated<TEntity>(items, count, pageNumber.Value, pageSize.Value);
    }

    /// <inheritdoc cref="IReadonlyRepository{TEntity,TPrimaryKey}.GetAllPaginatedAsync(System.Linq.IQueryable{TEntity},int,int,System.Threading.CancellationToken,string,string,int)"/>
    public async Task<Paginated<TEntity>> GetAllPaginatedAsync(int take, int skip,
        List<Expression<Func<TEntity, object>>>? includeExpressions = null,
        CancellationToken cancellationToken = default,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0
    )
    {
        var tag = GetQueryTag(callerMemberName, callerFilePath, callerLineNumber);

        var query = _dbSet.AsQueryable()
            .TagWith(tag);

        if (includeExpressions is not null && includeExpressions.Count is not 0)
        {
            query = includeExpressions.Aggregate(query,
                (current, includeExpression) => current.Include(includeExpression));
        }

        var count = await _dbSet.CountAsync(cancellationToken);

        var items = await query
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return new Paginated<TEntity>(items, count);
    }

    /// <inheritdoc cref="IReadonlyRepository{TEntity,TPrimaryKey}.GetAllPaginatedAsync(System.Linq.IQueryable{TEntity},int,int,System.Threading.CancellationToken,string,string,int)"/>
    public async Task<Paginated<TEntity>> GetAllPaginatedAsync(
        int take,
        int skip,
        Expression<Func<TEntity, bool>> expression,
        List<Expression<Func<TEntity, object>>>? includeExpressions = null,
        CancellationToken cancellationToken = default,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0
    )
    {
        var tag = GetQueryTag(callerMemberName, callerFilePath, callerLineNumber);

        var query = _dbSet.AsQueryable()
            .TagWith(tag);

        if (includeExpressions is { Count: > 0 })
        {
            query = includeExpressions.Aggregate(query,
                (current, includeExpression) => current.Include(includeExpression));
        }

        query = query.Where(expression);

        var count = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return new Paginated<TEntity>(items, count);
    }

    /// <inheritdoc cref="IReadonlyRepository{TEntity,TPrimaryKey}.GetByConditionAsync"/>
    public Task<TEntity?> GetByConditionAsync(
        Expression<Func<TEntity, bool>> expression,
        List<Expression<Func<TEntity, object>>>? includeExpressions = null,
        CancellationToken cancellationToken = default,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0
    )
    {
        var tag = GetQueryTag(callerMemberName, callerFilePath, callerLineNumber);

        var query = _dbSet
            .TagWith(tag)
            .Where(expression);

        if (includeExpressions != null && includeExpressions.Count != 0)
        {
            query = includeExpressions.Aggregate(query,
                (current, includeExpression) => current.Include(includeExpression));
        }

        return query.FirstOrDefaultAsync(cancellationToken);
    }

    private string GetQueryTag(string callerMemberName, string callerFilePath, int callerLineNumber)
    {
        return
            $"Caller: {GetType()}.{callerMemberName} - File: {Path.GetFileNameWithoutExtension(callerFilePath)} - Line: {callerLineNumber}";
    }
}