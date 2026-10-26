using Microsoft.EntityFrameworkCore.Storage;

namespace Fusion.Persistence.DB.Contract;

/// <summary>
/// The database interface to implement it and use its database object.
/// </summary>
public interface IServiceDb
{
    /// <summary>
    /// Call it if you want to execute something using database and transact.
    /// </summary>
    /// <param name="action">Action of transaction</param>
    /// <param name="cancellationToken">The cancellation token to cancel long-running Db tasks</param>
    Task Transaction(Func<Task> action, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the execution strategy for the current database context.
    /// </summary>
    /// <returns>An execution strategy instance tailored for retrying database operations.</returns>
    IExecutionStrategy GetExecutionStrategy();

    /// <summary>
    /// Begins a database transaction asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation, containing the initiated database transaction.</returns>
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Commits the given database transaction asynchronously.
    /// </summary>
    /// <param name="transaction">The database transaction to commit.</param>
    /// <param name="cancellationToken">The cancellation token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken);

    /// <summary>
    /// Rolls back the specified database transaction asynchronously.
    /// </summary>
    /// <param name="transaction">The database transaction to roll back.</param>
    /// <param name="cancellationToken">The cancellation token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RollbackTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken);

    /// <summary>
    /// Saves all changes made in the database context to the underlying database asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}