using Microsoft.Extensions.Logging;
using Quartz;

namespace ScheduledTask.Infrastructure.Listener;

public class JobListener : IJobListener
{
    private readonly ILogger<JobListener> logger;

    public string Name => "DefaultJobListener";

    public JobListener(ILogger<JobListener> logger)
    {
        this.logger = logger;
    }

    public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        logger.LogWarning("Job execution vetoed: {JobKey}", context.JobDetail.Key);
        return Task.CompletedTask;
    }

    public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Job about to execute: {JobKey}", context.JobDetail.Key);
        return Task.CompletedTask;
    }

    public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException? jobException, CancellationToken cancellationToken = default)
    {
        if (jobException == null)
        {
            logger.LogInformation("Job executed successfully: {JobKey}, Duration: {Duration}ms",
                context.JobDetail.Key, context.JobRunTime.TotalMilliseconds);
        }
        else
        {
            logger.LogError(jobException, "Job execution failed: {JobKey}, Duration: {Duration}ms",
                context.JobDetail.Key, context.JobRunTime.TotalMilliseconds);
        }
        return Task.CompletedTask;
    }
}
