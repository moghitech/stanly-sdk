using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Fusion.Entities;
using Fusion.Microservice.Models;

namespace Fusion.Persistence.Repositories.Contracts;

/// <summary>
/// Defines a read-only repository interface for querying entities of type TEntity with a primary key of type TPrimaryKey.
/// This interface provides methods for checking existence, retrieving entities by conditions or ID, and fetching paginated results.
/// It supports asynchronous operations, cancellation tokens, eager loading of related data, and caller information for debugging/logging.
/// </summary>
/// <typeparam name="TEntity">The type of the entity, which must implement IEntity<TPrimaryKey/>.</typeparam>
/// <typeparam name="TPrimaryKey">The type of the primary key for the entity.</typeparam>
public interface IReadonlyRepository<TEntity, in TPrimaryKey> where TEntity : IEntity<TPrimaryKey>
{
    /// <summary>
    /// Asynchronously checks if any entity matching the specified predicate exists in the data source.
    /// </summary>
    /// <param name="predicate">The condition to check for existence.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <param name="callerMemberName">Name of the calling method (for logging/debugging).</param>
    /// <param name="callerFilePath">File path of the calling code (for logging/debugging).</param>
    /// <param name="callerLineNumber">Line number of the calling code (for logging/debugging).</param>
    /// <returns>True if an entity exists matching the predicate; otherwise, false.</returns>
    Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0
    );

    /// <summary>
    /// Retrieves a queryable collection of entities based on the specified condition without tracking changes.
    /// Useful for read-only operations to improve performance.
    /// </summary>
    /// <param name="expression">The condition to filter entities.</param>
    /// <param name="includeExpressions">Optional list of related data to eager load.</param>
    /// <param name="callerMemberName">Name of the calling method (for logging/debugging).</param>
    /// <param name="callerFilePath">File path of the calling code (for logging/debugging).</param>
    /// <param name="callerLineNumber">Line number of the calling code (for logging/debugging).</param>
    /// <returns>An IQueryable of entities matching the condition.</returns>
    IQueryable<TEntity> GetAsNoTracking(
        Expression<Func<TEntity, bool>> expression,
        List<Expression<Func<TEntity, object>>>? includeExpressions = null,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0
    );

    /// <summary>
    /// Asynchronously retrieves all entities matching the specified condition.
    /// </summary>
    /// <param name="expression">The condition to filter entities.</param>
    /// <param name="includeExpressions">Optional list of related data to eager load.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <param name="callerMemberName">Name of the calling method (for logging/debugging).</param>
    /// <param name="callerFilePath">File path of the calling code (for logging/debugging).</param>
    /// <param name="callerLineNumber">Line number of the calling code (for logging/debugging).</param>
    /// <returns>A collection of entities matching the condition.</returns>
    Task<IEnumerable<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>> expression,
        List<Expression<Func<TEntity, object>>>? includeExpressions = null,
        CancellationToken cancellationToken = default,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0
    );

    /// <summary>
    /// Retrieves a queryable collection of all entities, optionally including related data.
    /// </summary>
    /// <param name="includeExpression">Optional list of related data to eager load.</param>
    /// <param name="callerMemberName">Name of the calling method (for logging/debugging).</param>
    /// <param name="callerFilePath">File path of the calling code (for logging/debugging).</param>
    /// <param name="callerLineNumber">Line number of the calling code (for logging/debugging).</param>
    /// <returns>An IQueryable of all entities.</returns>
    IQueryable<TEntity> GetAllAsQuery(
        List<Expression<Func<TEntity, object>>>? includeExpression = null,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0
    );

    /// <summary>
    /// Asynchronously retrieves an entity by its primary key.
    /// </summary>
    /// <param name="id">The primary key of the entity to retrieve.</param>
    /// <param name="includeExpression">Optional list of related data to eager load.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <param name="callerMemberName">Name of the calling method (for logging/debugging).</param>
    /// <param name="callerFilePath">File path of the calling code (for logging/debugging).</param>
    /// <param name="callerLineNumber">Line number of the calling code (for logging/debugging).</param>
    /// <returns>The entity if found; otherwise, null.</returns>
    Task<TEntity?> GetByIdAsync(
        TPrimaryKey id,
        List<Expression<Func<TEntity, object>>>? includeExpression = null,
        CancellationToken cancellationToken = default,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0
    );

    /// <summary>
    /// Asynchronously retrieves the first entity matching the specified condition.
    /// </summary>
    /// <param name="expression">The condition to filter the entity.</param>
    /// <param name="includeExpression">Optional list of related data to eager load.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <param name="callerMemberName">Name of the calling method (for logging/debugging).</param>
    /// <param name="callerFilePath">File path of the calling code (for logging/debugging).</param>
    /// <param name="callerLineNumber">Line number of the calling code (for logging/debugging).</param>
    /// <returns>The first matching entity if found; otherwise, null.</returns>
    Task<TEntity?> GetByConditionAsync(
        Expression<Func<TEntity, bool>> expression,
        List<Expression<Func<TEntity, object>>>? includeExpression = null,
        CancellationToken cancellationToken = default,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0
    );

    /// <summary>
    /// Asynchronously retrieves a paginated list of entities from a provided query.
    /// </summary>
    /// <param name="query">The query to paginate.</param>
    /// <param name="take">Number of entities to take per page.</param>
    /// <param name="skip">Number of entities to skip.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <param name="callerMemberName">Name of the calling method (for logging/debugging).</param>
    /// <param name="callerFilePath">File path of the calling code (for logging/debugging).</param>
    /// <param name="callerLineNumber">Line number of the calling code (for logging/debugging).</param>
    /// <returns>A list of paginated entities.</returns>
    Task<List<TEntity>> GetAllPaginatedAsync(
        IQueryable<TEntity> query,
        int take,
        int skip,
        CancellationToken cancellationToken = default,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0
    );

    /// <summary>
    /// Asynchronously retrieves a paginated result of all entities.
    /// </summary>
    /// <param name="take">Number of entities to take per page.</param>
    /// <param name="skip">Number of entities to skip.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <param name="callerMemberName">Name of the calling method (for logging/debugging).</param>
    /// <param name="callerFilePath">File path of the calling code (for logging/debugging).</param>
    /// <param name="callerLineNumber">Line number of the calling code (for logging/debugging).</param>
    /// <returns>A paginated result of entities.</returns>
    Task<Paginated<TEntity>> GetAllPaginatedAsync(
        int take,
        int skip,
        CancellationToken cancellationToken,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0
    );

    /// <summary>
    /// Asynchronously retrieves a paginated result of entities based on a condition.
    /// </summary>
    /// <param name="pageSize">Number of entities per page (optional).</param>
    /// <param name="pageNumber">Page number to retrieve (optional).</param>
    /// <param name="expression">Optional condition to filter entities.</param>
    /// <param name="includeExpressions">Optional list of related data to eager load.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <param name="callerMemberName">Name of the calling method (for logging/debugging).</param>
    /// <param name="callerFilePath">File path of the calling code (for logging/debugging).</param>
    /// <param name="callerLineNumber">Line number of the calling code (for logging/debugging).</param>
    /// <returns>A paginated result of entities.</returns>
    Task<Paginated<TEntity>> GetAllPaginatedAsync(
        int? pageSize,
        int? pageNumber,
        Expression<Func<TEntity, bool>>? expression,
        List<Expression<Func<TEntity, object>>>? includeExpressions,
        CancellationToken cancellationToken,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0
    );

    /// <summary>
    /// Asynchronously retrieves a paginated result of entities from a provided query.
    /// </summary>
    /// <param name="query">The query to paginate.</param>
    /// <param name="pageSize">Number of entities per page (optional).</param>
    /// <param name="pageNumber">Page number to retrieve (optional).</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <param name="callerMemberName">Name of the calling method (for logging/debugging).</param>
    /// <param name="callerFilePath">File path of the calling code (for logging/debugging).</param>
    /// <param name="callerLineNumber">Line number of the calling code (for logging/debugging).</param>
    /// <returns>A paginated result of entities.</returns>
    Task<Paginated<TEntity>> GetAllPaginatedAsync(
        IQueryable<TEntity> query,
        int? pageSize,
        int? pageNumber,
        CancellationToken cancellationToken,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0
    );

    /// <summary>
    /// Asynchronously retrieves a paginated result of all entities with specified take and skip values.
    /// </summary>
    /// <param name="take">Number of entities to take per page.</param>
    /// <param name="skip">Number of entities to skip.</param>
    /// <param name="includeExpressions">Optional list of related data to eager load.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <param name="callerMemberName">Name of the calling method (for logging/debugging).</param>
    /// <param name="callerFilePath">File path of the calling code (for logging/debugging).</param>
    /// <param name="callerLineNumber">Line number of the calling code (for logging/debugging).</param>
    /// <returns>A paginated result of entities.</returns>
    Task<Paginated<TEntity>> GetAllPaginatedAsync(
        int take,
        int skip,
        List<Expression<Func<TEntity, object>>>? includeExpressions = null,
        CancellationToken cancellationToken = default,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0
    );

    /// <summary>
    /// Asynchronously retrieves a paginated result of entities based on a condition with specified take and skip values.
    /// </summary>
    /// <param name="take">Number of entities to take per page.</param>
    /// <param name="skip">Number of entities to skip.</param>
    /// <param name="expression">Condition to filter entities.</param>
    /// <param name="includeExpressions">Optional list of related data to eager load.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <param name="callerMemberName">Name of the calling method (for logging/debugging).</param>
    /// <param name="callerFilePath">File path of the calling code (for logging/debugging).</param>
    /// <param name="callerLineNumber">Line number of the calling code (for logging/debugging).</param>
    /// <returns>A paginated result of entities.</returns>
    Task<Paginated<TEntity>> GetAllPaginatedAsync(
        int take,
        int skip,
        Expression<Func<TEntity, bool>> expression,
        List<Expression<Func<TEntity, object>>>? includeExpressions = null,
        CancellationToken cancellationToken = default,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0
    );
}