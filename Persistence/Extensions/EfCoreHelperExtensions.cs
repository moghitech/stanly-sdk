using Fusion.Exceptions.RuntimeExceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fusion.Persistence.Extensions;

/// <summary>
/// Ef code registration functions and helpers.
/// </summary>
public static class EfCoreHelperExtensions
{
    /// <summary>
    /// Add EF Core configuration according to use Postgres as DB. 
    /// </summary>
    /// <param name="services">The services to inject context.</param>
    /// <param name="connectionString">Connection string of DB</param>
    /// <param name="timeout">The time-out to leave from connecting to DB server.</param>
    /// <typeparam name="TContext">The expected DB Context</typeparam>
    private static void AddEfCorePostgresDbContext<TContext>(this IServiceCollection services, string connectionString,
        int timeout = 30) where TContext : DbContext
    {
        services.AddDbContextPool<TContext>(options =>
        {
            options.UseNpgsql(connectionString, optionsBuilder =>
            {
                optionsBuilder.EnableRetryOnFailure();
                optionsBuilder.CommandTimeout(timeout);
            });
        });

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    }

    /// <summary>
    /// Adds connection string and configure Db-Context according to configuration.
    /// </summary>
    /// <param name="services">The services to use</param>
    /// <param name="config">The configuration of context</param>
    /// <param name="connectionName">The connection name exist settings based files like appsettings.json</param>
    /// <typeparam name="TContext">Type of context</typeparam>
    /// <exception cref="ConnectionStringException">Exception when something wrong with connection string.</exception>
    public static void AddConnectionStrings<TContext>(this IServiceCollection services, IConfigurationRoot config,
        string connectionName = "Default")
        where TContext : DbContext
    {
        var connectionString = config.GetConnectionString(connectionName);

        if (connectionString is null)
            throw new ConnectionStringException(connectionName);

        services.AddEfCorePostgresDbContext<TContext>(connectionString);
    }

    /// <summary>
    /// Migrate all changes from migrations folder.
    /// </summary>
    /// <param name="services">The services to use</param>
    public static void UpdateMigrations<TContext>(this IServiceCollection services) where TContext : DbContext
    {
        using var scope = services.BuildServiceProvider().CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();
        dbContext.Database.Migrate();
    }
}