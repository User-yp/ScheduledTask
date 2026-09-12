using Microsoft.Extensions.Diagnostics.HealthChecks;
using ScheduledTask.Dll.Redis;

namespace ScheduledTask.WebApi.HealthChecks;

public class RedisHealthCheck : IHealthCheck
{
    private readonly IRedisService redis;

    public RedisHealthCheck(IRedisService redis)
    {
        this.redis = redis;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var isHealthy = await redis.PingAsync();
        if (isHealthy)
        {
            return HealthCheckResult.Healthy("Redis 连接正常");
        }
        return HealthCheckResult.Unhealthy("Redis 连接不可达");
    }
}
