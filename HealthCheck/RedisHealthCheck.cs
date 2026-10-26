using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace Fusion.HealthCheck;

/// <summary>
/// Implements a health check for a Redis server.
/// </summary>
/// <remarks>
/// The <see cref="RedisHealthCheck"/> class checks the health of a Redis server
/// using a simple ping operation executed on the default database.
/// </remarks>
public class RedisHealthCheck(IConnectionMultiplexer connection) : IHealthCheck
{
    /// <summary>
    /// Checks the health status of the Redis connection.
    /// </summary>
    /// <param name="context">The context in which the health check is being performed.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the health check.</param>
    /// <returns>A <see cref="HealthCheckResult"/> representing the outcome of the health check.</returns>
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = new())
    {
        try
        {
            await connection.GetDatabase(0).PingAsync();
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(exception: ex);
        }
    }
}