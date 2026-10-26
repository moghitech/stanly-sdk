using Fusion.Persistence.DB.Contract;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Fusion.Persistence.DB;

/// <inheritdoc cref= "IServiceDb"/>
public sealed class ServiceDb<TContext>(TContext dbContext, ILogger<ServiceDb<TContext>> logger)
    : IServiceDb where TContext : DbContext
{
    /// <inheritdoc cref= "IServiceDb.Transaction"/>
    public async Task Transaction(Func<Task> action, CancellationToken cancellationToken)
    {
        var executionStrategy = dbContext.Database.CreateExecutionStrategy();

        await executionStrategy.Execute(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await action.Invoke();

                logger.LogInformation("Transaction action performed successfully.");

                await transaction.CommitAsync(cancellationToken);

                logger.LogInformation("Transaction commited successfully.");
            }
            catch (Exception ex)
            {
                logger.LogCritical("Transaction Failed with [Message]=({message})", ex.Message);

                await transaction.RollbackAsync(cancellationToken);
            }
        });
    }

    /// <inheritdoc cref= "IServiceDb.GetExecutionStrategy"/>
    public IExecutionStrategy GetExecutionStrategy()
    {
        return dbContext.Database.CreateExecutionStrategy();
    }

    /// <inheritdoc cref= "IServiceDb.BeginTransactionAsync"/>
    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    /// <inheritdoc cref= "IServiceDb.CommitTransactionAsync"/>
    public async Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken)
    {
        await transaction.CommitAsync(cancellationToken);
    }

    /// <inheritdoc cref= "IServiceDb.RollbackTransactionAsync"/>
    public async Task RollbackTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken)
    {
        await transaction.RollbackAsync(cancellationToken);
    }

    /// <inheritdoc cref= "IServiceDb.SaveChangesAsync"/>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await dbContext.SaveChangesAsync(cancellationToken);
    }
}