using Fusion.Caching;
using Fusion.Caching.Internals;
using Fusion.Exceptions.RuntimeExceptions;
using Fusion.Microservice.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Fusion.Microservice.Extensions.ORM;

/// <summary>
/// Redis Service Registration
/// </summary>
public static class RedisServiceRegistration
{
    /// <summary>
    /// Registers Redis cache services in the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to which the Redis cache services will be added.</param>
    /// <param name="configuration">The application configuration instance used to read Redis settings.</param>
    /// <exception cref="ConnectionStringException">Thrown when the connection string for Redis is missing or invalid.</exception>
    public static async Task AddRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        var redisOptions = configuration.GetServiceOptions().RedisOptions;

        if (redisOptions.IsSentinel)
        {
            var sentinelMasterConnection = await GetSentinelMasterConnectionAsync(redisOptions);
            services.AddSingleton<IConnectionMultiplexer>(sentinelMasterConnection);
        }
        else if (!string.IsNullOrWhiteSpace(redisOptions.Password))
        {
            var masterConfig = new ConfigurationOptions
            {
                Password = redisOptions.Password,
                AbortOnConnectFail = false,
                SyncTimeout = 60000,
                ConnectTimeout = 10000,
                KeepAlive = 60,
                ReconnectRetryPolicy = new ExponentialRetry(2000)
            };
            masterConfig.EndPoints.Add(redisOptions.ConnectionString);
            var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(masterConfig);
            services.AddSingleton<IConnectionMultiplexer>(_ => connectionMultiplexer);
        }
        else
        {
            var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(redisOptions.ConnectionString);
            services.AddSingleton<IConnectionMultiplexer>(_ => connectionMultiplexer);
        }

        services.AddSingleton<IRedisCacheProvider, RedisCacheProvider>();
        services.AddSingleton<ICacheProvider, RedisCacheProvider>();
    }

    private static async Task<ConnectionMultiplexer> GetSentinelMasterConnectionAsync(RedisOptions redisOptions)
    {
        var sentinelConfig = new ConfigurationOptions
        {
            TieBreaker = "",
            CommandMap = CommandMap.Sentinel,
            AbortOnConnectFail = false,
            Password = redisOptions.Password
        };

        var sentinelEndpoints = redisOptions.ConnectionString.Split(',');
        foreach (var endpoint in sentinelEndpoints)
            sentinelConfig.EndPoints.Add(endpoint);

        await using var sentinelConnection = await ConnectionMultiplexer.ConnectAsync(sentinelConfig);
        if (!sentinelConnection.IsConnected)
            throw new RedisException(
                $"Unable to connect to Redis Sentinel. [Connection String]=({redisOptions.ConnectionString}");

        var server = sentinelConnection.GetServer(sentinelEndpoints[0]);
        var masterEndpoint = await server.SentinelGetMasterAddressByNameAsync(redisOptions.ServiceName);
        if (masterEndpoint is null)
            throw new RedisException($"Could not locate master for service '{redisOptions.ServiceName}'.");

        var masterConfig = new ConfigurationOptions
        {
            Password = redisOptions.Password,
            AbortOnConnectFail = false,
            SyncTimeout = 60000,
            ConnectTimeout = 60000,
            KeepAlive = 60,
            ReconnectRetryPolicy = new ExponentialRetry(2000)
        };
        masterConfig.EndPoints.Add(masterEndpoint);

        var masterConnection = await ConnectionMultiplexer.ConnectAsync(masterConfig);
        if (!masterConnection.IsConnected)
            throw new RedisException($"Unable to connect to the Redis master. [Master Endpoint]=({masterEndpoint})");

        return masterConnection;
    }
}