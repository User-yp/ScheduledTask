using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using ScheduledTask.Domain.IRepository;
using ScheduledTask.Domain.Persistence;
namespace ScheduledTask.Infrastructure.Listener;

public class TriggerListener : ITriggerListener
{
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly ILogger<TriggerListener> logger;

    public string Name => "DefaultTriggerListener";

    public TriggerListener(IServiceScopeFactory serviceScopeFactory, ILogger<TriggerListener> logger)
    {
        this.serviceScopeFactory = serviceScopeFactory;
        this.logger = logger;
    }

    public async Task TriggerComplete(ITrigger trigger, IJobExecutionContext context, SchedulerInstruction triggerInstructionCode, CancellationToken cancellationToken = default)
    {
        try
        {
            var startTime = (DateTime)context.Get("StartTime");
            var duration = DateTime.Now - startTime;

            var record = new TriggerRecord
            {
                SchedulerName = context.Scheduler.SchedulerName,
                SchedulerId = context.Scheduler.SchedulerInstanceId,
                JobKey = context.JobDetail.Key.Name,
                TriggerKey = trigger.Key.Name,
                FireTime = startTime,
                Duration = (int)duration.TotalMilliseconds,
                Success = context.Result == null,
                ErrorMessage = context.Result?.ToString()
            };

            using var scope = serviceScopeFactory.CreateScope();
            var store = scope.ServiceProvider.GetRequiredService<ITriggerStore>();
            await store.AddRecordAsync(record, cancellationToken);

            // 执行失败时发送告警通知 + 自动重试
            if (!record.Success)
            {
                // 发送通知
                var notificationService = scope.ServiceProvider.GetService<INotificationService>();
                if (notificationService != null)
                {
                    _ = notificationService.SendFailureNotificationAsync(new NotificationMessage
                    {
                        JobKey = record.JobKey,
                        TriggerKey = record.TriggerKey,
                        FireTime = record.FireTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        Duration = record.Duration,
                        ErrorMessage = record.ErrorMessage,
                        SchedulerName = record.SchedulerName,
                        SchedulerId = record.SchedulerId
                    }, cancellationToken);
                }

                // 自动重试逻辑
                await TryScheduleRetryAsync(scope, context, trigger, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to record trigger completion for {TriggerKey}", trigger.Key);
        }
    }

    public Task TriggerFired(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        // 记录触发开始时间
        context.Put("StartTime", DateTime.Now);
        logger.LogDebug("Trigger fired: {TriggerKey}", trigger.Key);
        return Task.CompletedTask;
    }

    public Task TriggerMisfired(ITrigger trigger, CancellationToken cancellationToken = default)
    {
        logger.LogWarning("Trigger misfired: {TriggerKey}", trigger.Key);
        return Task.CompletedTask;
    }

    public Task<bool> VetoJobExecution(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        logger.LogWarning("Trigger veto check: {TriggerKey}", trigger.Key);
        return Task.FromResult(false);
    }

    /// <summary>
    /// 失败自动重试：根据 JobConfig 中的 MaxRetries 配置调度重试触发器
    /// </summary>
    private static async Task TryScheduleRetryAsync(IServiceScope scope, IJobExecutionContext context, ITrigger trigger, CancellationToken cancellationToken)
    {
        try
        {
            var retryCount = context.MergedJobDataMap.GetInt("__RetryCount__");
            if (retryCount == 0 && !context.MergedJobDataMap.ContainsKey("__RetryCount__"))
                retryCount = 0;

            // 从 JobDataMap 读取重试配置（由 JobConfig.JobData 注入）
            var maxRetries = context.MergedJobDataMap.GetInt("MaxRetries");
            var retryDelay = context.MergedJobDataMap.GetInt("RetryDelaySeconds");
            if (maxRetries <= 0 || retryDelay <= 0)
                return;

            retryCount++;
            if (retryCount > maxRetries)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<TriggerListener>>();
                logger.LogWarning("Max retries reached ({MaxRetries}) for job {JobKey}", maxRetries, context.JobDetail.Key);
                return;
            }

            var scheduler = scope.ServiceProvider.GetRequiredService<IScheduler>();
            var retryTrigger = TriggerBuilder.Create()
                .WithIdentity($"{trigger.Key.Name}_retry_{retryCount}", trigger.Key.Group)
                .ForJob(context.JobDetail.Key)
                .StartAt(DateTimeOffset.UtcNow.AddSeconds(retryDelay))
                .UsingJobData("__RetryCount__", retryCount)
                .Build();

            await scheduler.ScheduleJob(retryTrigger, cancellationToken);
        }
        catch (Exception)
        {
            // 重试失败不影响主流程
        }
    }
}
