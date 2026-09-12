using Microsoft.Extensions.Logging;
using Quartz;

namespace ScheduledTask.Infrastructure.Listener;

public class SchedulerListener : ISchedulerListener
{
    public string Name => "DefaultSchedulerListener";
    private readonly ILogger<SchedulerListener> logger;

    public SchedulerListener(ILogger<SchedulerListener> logger)
    {
        this.logger = logger;
    }

    public Task JobAdded(IJobDetail jobDetail, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Job added: {JobKey}, Description: {Description}", jobDetail.Key, jobDetail.Description);
        return Task.CompletedTask;
    }

    public Task JobDeleted(JobKey jobKey, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Job deleted: {JobKey}", jobKey);
        return Task.CompletedTask;
    }

    public Task JobInterrupted(JobKey jobKey, CancellationToken cancellationToken = default)
    {
        logger.LogWarning("Job interrupted: {JobKey}", jobKey);
        return Task.CompletedTask;
    }

    public Task JobPaused(JobKey jobKey, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Job paused: {JobKey}", jobKey);
        return Task.CompletedTask;
    }

    public Task JobResumed(JobKey jobKey, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Job resumed: {JobKey}", jobKey);
        return Task.CompletedTask;
    }

    public Task JobScheduled(ITrigger trigger, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Job scheduled, Trigger: {TriggerKey}", trigger.Key);
        return Task.CompletedTask;
    }

    public Task JobsPaused(string jobGroup, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Jobs paused in group: {JobGroup}", jobGroup);
        return Task.CompletedTask;
    }

    public Task JobsResumed(string jobGroup, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Jobs resumed in group: {JobGroup}", jobGroup);
        return Task.CompletedTask;
    }

    public Task JobUnscheduled(TriggerKey triggerKey, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Job unscheduled, Trigger: {TriggerKey}", triggerKey);
        return Task.CompletedTask;
    }

    public Task SchedulerError(string msg, SchedulerException cause, CancellationToken cancellationToken = default)
    {
        logger.LogError(cause, "Scheduler error: {ErrorMessage}", msg);
        return Task.CompletedTask;
    }

    public Task SchedulerInStandbyMode(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Scheduler entered standby mode");
        return Task.CompletedTask;
    }

    public Task SchedulerShutdown(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Scheduler shutdown");
        return Task.CompletedTask;
    }

    public Task SchedulerShuttingdown(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Scheduler shutting down");
        return Task.CompletedTask;
    }

    public Task SchedulerStarted(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Scheduler started");
        return Task.CompletedTask;
    }

    public Task SchedulerStarting(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Scheduler starting");
        return Task.CompletedTask;
    }

    public Task SchedulingDataCleared(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Scheduling data cleared");
        return Task.CompletedTask;
    }

    public Task TriggerFinalized(ITrigger trigger, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Trigger finalized: {TriggerKey}", trigger.Key);
        return Task.CompletedTask;
    }

    public Task TriggerPaused(TriggerKey triggerKey, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Trigger paused: {TriggerKey}", triggerKey);
        return Task.CompletedTask;
    }

    public Task TriggerResumed(TriggerKey triggerKey, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Trigger resumed: {TriggerKey}", triggerKey);
        return Task.CompletedTask;
    }

    public Task TriggersPaused(string? triggerGroup, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Triggers paused in group: {TriggerGroup}", triggerGroup);
        return Task.CompletedTask;
    }

    public Task TriggersResumed(string? triggerGroup, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Triggers resumed in group: {TriggerGroup}", triggerGroup);
        return Task.CompletedTask;
    }
}
