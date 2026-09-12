using Microsoft.Extensions.Diagnostics.HealthChecks;
using Quartz;

namespace ScheduledTask.WebApi.HealthChecks;

public class QuartzHealthCheck : IHealthCheck
{
    private readonly IScheduler scheduler;

    public QuartzHealthCheck(IScheduler scheduler)
    {
        this.scheduler = scheduler;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (scheduler.IsStarted)
        {
            return Task.FromResult(HealthCheckResult.Healthy("Quartz 调度器正在运行"));
        }
        return Task.FromResult(HealthCheckResult.Unhealthy("Quartz 调度器未启动"));
    }
}
