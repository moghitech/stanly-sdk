using Fusion.Exceptions.RuntimeExceptions;
using Fusion.Messaging.RabbitMq.Integration.Interceptors;
using Fusion.Microservice.Extensions.Options;
using Fusion.Persistence.DB;
using Fusion.Persistence.DB.Contract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Fusion.Microservice.Extensions.ORM;

/// <summary>
/// Provides methods to register a PostgreSQL database context pool in the service collection.
/// </summary>
public static class NpgServiceRegistration
{
    /// <summary>
    /// Registers a PostgreSQL database context pool with the specified options, enabling optional integration interceptor injection.
    /// </summary>
    /// <typeparam name="TContext">The type of the DbContext to register.</typeparam>
    /// <param name="services">The dependency injection service collection to add the context pool to.</param>
    /// <param name="npgOptionsAction">A delegate to configure the PostgreSQL connection options.</param>
    /// <param name="injectIntegrationInterceptor">
    /// A boolean value indicating whether to inject an integration interceptor into the database context configuration.
    /// Defaults to false.
    /// </param>
    /// <exception cref="ConnectionStringException">
    /// Thrown when the connection string is null, empty, or whitespace in the provided options.
    /// </exception>
    public static void AddNpgSqlDbContextPool<TContext>(
        this IServiceCollection services,
        Action<NpgOptions> npgOptionsAction,
        bool injectIntegrationInterceptor = false
    ) where TContext : DbContext
    {
        var npgOptions = new NpgOptions();
        npgOptionsAction.Invoke(npgOptions);

        if (string.IsNullOrWhiteSpace(npgOptions.ConnectionString))
            throw new ConnectionStringException(nameof(npgOptions.ConnectionString));

        services.AddDbContextPool<TContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(npgOptions.ConnectionString, optionsBuilder =>
            {
                optionsBuilder.EnableRetryOnFailure(
                    maxRetryCount: npgOptions.MaxRetryOnFailureCount,
                    maxRetryDelay: TimeSpan.FromSeconds(npgOptions.MaxRetryOnFailureDelayBySeconds),
                    errorCodesToAdd: null
                );
                optionsBuilder.CommandTimeout(npgOptions.CommandTimeout);
            });
            options.UseLazyLoadingProxies(npgOptions.LazyLoading);
            options.UseQueryTrackingBehavior(npgOptions.TrackingBehavior);

            if (injectIntegrationInterceptor)
            {
                using var scope = serviceProvider.CreateAsyncScope();
                var interceptor = scope.ServiceProvider.GetRequiredService<IntegrationEventPublishingInterceptor>();
                options.AddInterceptors(interceptor);
            }
        });

        services.AddScoped<IServiceDb, ServiceDb<TContext>>();
    }
}