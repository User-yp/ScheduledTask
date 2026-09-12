using Microsoft.EntityFrameworkCore;
using Quartz.Impl.Matchers;
using Quartz;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ITrigger = Quartz.ITrigger;
using ScheduledTask.Domain.IRepository;
using ScheduledTask.Domain.Persistence;
using ScheduledTask.Infrastructure.DbContexts;

namespace ScheduledTask.Infrastructure.Repository;

public class SchedulerManager : ISchedulerManager
{
    private readonly QuartzContext quartzContext;
    private readonly ITriggerStore store;
    private readonly IScheduler scheduler;
    private readonly ILogger<SchedulerManager> logger;

    public SchedulerManager(IServiceProvider serviceProvider)
    {
        quartzContext = serviceProvider.GetRequiredService<QuartzContext>();
        store = serviceProvider.GetRequiredService<ITriggerStore>();
        scheduler = serviceProvider.GetRequiredService<IScheduler>()
            ?? throw new ArgumentNullException(nameof(IScheduler));
        logger = serviceProvider.GetRequiredService<ILogger<SchedulerManager>>();
    }

    #region Group
    public async Task<List<string>?> GetAllGroupAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var jobGroups = await scheduler.GetJobGroupNames(cancellationToken);
            return jobGroups.ToList();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get all job groups");
            return null;
        }
    }
    #endregion

    #region Job
    // 查看所有作业
    public async Task<List<JobKey>?> GetAllJobsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var jobGroups = await scheduler.GetJobGroupNames(cancellationToken);
            var jobs = new List<JobKey>();

            foreach (var group in jobGroups)
            {
                var jobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(group), cancellationToken);
                jobs.AddRange(jobKeys);
            }
            return jobs;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get all jobs");
            return null;
        }
    }

    public async Task<List<JobKey>?> GetJobsByGroupAsync(string groupName, CancellationToken cancellationToken = default)
    {
        try
        {
            var jobs = new List<JobKey>();
            if (string.IsNullOrEmpty(groupName))
                return jobs;
            var jobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(groupName), cancellationToken);
            foreach (var jobKey in jobKeys)
            {
                jobs.Add(jobKey);
            }
            return jobs;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get jobs by group '{GroupName}'", groupName);
            return null;
        }
    }

    public async Task<JobKey?> GetJobAsync(string jobName, CancellationToken cancellationToken = default)
    {
        try
        {
            var jobs = await GetAllJobsAsync(cancellationToken);
            var job = jobs?.FirstOrDefault(j => j.Name == jobName);
            return job;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get job '{JobName}'", jobName);
            return null;
        }
    }

    // 暂停所有作业
    public async Task<bool> PauseAllJobAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await scheduler.PauseAll(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to pause all jobs");
            return false;
        }
    }

    // 暂停指定作业
    public async Task<bool> PauseJobAsync(JobKey jobKey, CancellationToken cancellationToken = default)
    {
        try
        {
            if (jobKey != null)
            {
                if (await scheduler.CheckExists(jobKey, cancellationToken))
                {
                    await scheduler.PauseJob(jobKey, cancellationToken);
                    return true;
                }
            }
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to pause job '{JobKey}'", jobKey);
            return false;
        }
    }

    // 批量暂停作业
    public async Task<bool> PauseJobsAsync(List<JobKey> jobKeys, CancellationToken cancellationToken = default)
    {
        try
        {
            if (jobKeys.Count != 0)
            {
                foreach (var jobKey in jobKeys)
                {
                    await PauseJobAsync(jobKey, cancellationToken);
                }
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to pause jobs batch");
            return false;
        }
    }

    // 按组暂停作业
    public async Task<bool> PauseJobsByGroupAsync(string groupName, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(groupName))
                return false;

            var jobKeys = await GetJobsByGroupAsync(groupName, cancellationToken);

            if (jobKeys?.Count > 0)
            {
                return await PauseJobsAsync(jobKeys);
            }
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to pause jobs by group '{GroupName}'", groupName);
            return false;
        }
    }

    // 恢复所有作业
    public async Task<bool> ResumeAllJobAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await scheduler.ResumeAll();
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to resume all jobs");
            return false;
        }
    }

    // 恢复指定作业
    public async Task<bool> ResumeJobAsync(JobKey jobKey, CancellationToken cancellationToken = default)
    {
        try
        {
            if (jobKey != null)
            {
                if (await scheduler.CheckExists(jobKey, cancellationToken))
                {
                    await scheduler.ResumeJob(jobKey, cancellationToken);
                    return true;
                }
            }
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to resume job '{JobKey}'", jobKey);
            return false;
        }
    }

    // 批量恢复作业
    public async Task<bool> ResumeJobsAsync(List<JobKey> jobKeys, CancellationToken cancellationToken = default)
    {
        try
        {
            if (jobKeys.Count != 0)
            {
                foreach (var jobKey in jobKeys)
                {
                    await ResumeJobAsync(jobKey, cancellationToken);
                }
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to resume jobs batch");
            return false;
        }
    }

    // 按组恢复作业
    public async Task<bool> ResumeJobsByGroupAsync(string groupName, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(groupName))
                return false;

            var jobKeys = await GetJobsByGroupAsync(groupName, cancellationToken);

            if (jobKeys?.Count > 0)
                return await ResumeJobsAsync(jobKeys, cancellationToken);

            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to resume jobs by group '{GroupName}'", groupName);
            return false;
        }
    }

    // 立即执行作业
    public async Task<bool> TriggerJobAsync(JobKey jobKey, CancellationToken cancellationToken = default)
    {
        try
        {
            if (await scheduler.CheckExists(jobKey, cancellationToken))
            {
                await scheduler.TriggerJob(jobKey, cancellationToken);
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to trigger job '{JobKey}'", jobKey);
            return false;
        }
    }

    // 批量立即执行作业
    public async Task<bool> TriggerJobsAsync(List<JobKey> jobKeys, CancellationToken cancellationToken = default)
    {
        try
        {
            if (jobKeys.Count == 0)
                return false;
            foreach (var jobKey in jobKeys)
            {
                await TriggerJobAsync(jobKey, cancellationToken);
            }
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to trigger jobs batch");
            return false;
        }
    }

    // 清除所有作业
    public async Task<bool> DeleteAllJobAsync(JobKey jobKey, CancellationToken cancellationToken = default)
    {
        try
        {
            await scheduler.Clear(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to clear all jobs");
            return false;
        }
    }

    // 删除作业
    public async Task<bool> DeleteJobAsync(JobKey jobKey, CancellationToken cancellationToken = default)
    {
        try
        {
            if (jobKey == null)
                return false;

            if (await scheduler.CheckExists(jobKey, cancellationToken))
            {
                await scheduler.DeleteJob(jobKey, cancellationToken);
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete job '{JobKey}'", jobKey);
            return false;
        }
    }

    // 批量删除作业
    public async Task<bool> DeleteJobsAsync(List<JobKey> jobKeys, CancellationToken cancellationToken = default)
    {
        try
        {
            if (jobKeys.Count != 0)
            {
                foreach (var jobKey in jobKeys)
                {
                    await DeleteJobAsync(jobKey, cancellationToken);
                }
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete jobs batch");
            return false;
        }
    }

    // 按组删除作业
    public async Task<bool> DeleteJobByGroupAsync(string groupName, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(groupName))
                return false;

            var jobKeys = await GetJobsByGroupAsync(groupName, cancellationToken);

            if (jobKeys?.Count > 0)
                return await DeleteJobsAsync(jobKeys, cancellationToken);

            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete jobs by group '{GroupName}'", groupName);
            return false;
        }
    }

    #endregion

    #region Trigger

    public async Task<ITrigger?> GetTriggerAsync(TriggerKey triggerKey, CancellationToken cancellationToken = default)
    {
        try
        {
            var trigger = await scheduler.GetTrigger(triggerKey, cancellationToken);
            return trigger;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get trigger '{TriggerKey}'", triggerKey);
            return null;
        }
    }

    public async Task<List<ITrigger>?> GetTriggersOfJobAsync(JobKey jobKey, CancellationToken cancellationToken = default)
    {
        try
        {
            var triggers = await scheduler.GetTriggersOfJob(jobKey, cancellationToken);
            return triggers?.ToList();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get triggers of job '{JobKey}'", jobKey);
            return null;
        }
    }

    public async Task<TriggerState?> GetTriggerStateAsync(TriggerKey triggerKey, CancellationToken cancellationToken = default)
    {
        try
        {
            var state = await scheduler.GetTriggerState(triggerKey, cancellationToken);
            return state;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get trigger state '{TriggerKey}'", triggerKey);
            return null;
        }
    }
    #endregion

    #region JobDetail
    public async Task<IJobDetail?> GetJobDetailAsync(JobKey jobKey, CancellationToken cancellationToken = default)
    {
        try
        {
            var detail = await scheduler.GetJobDetail(jobKey, cancellationToken);
            return detail;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get job detail '{JobKey}'", jobKey);
            return null;
        }
    }

    public async Task<IJobDetail?> CreateJobDetailAsync(JobKey jobKey, CancellationToken cancellationToken = default)
    {
        try
        {
            var detail = await scheduler.GetJobDetail(jobKey, cancellationToken);
            return detail;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create job detail '{JobKey}'", jobKey);
            return null;
        }
    }

    public async Task<bool> AddTriggerToJobAsync(JobKey jobKey, string cron, CancellationToken cancellationToken = default)
    {
        try
        {
            // 验证 Job 是否存在
            if (!await scheduler.CheckExists(jobKey, cancellationToken))
                return false;
            var newTrigger = TriggerBuilder.Create()
                .WithIdentity(new TriggerKey(Guid.NewGuid().ToString(), jobKey.Group))
                .WithCronSchedule(cron, builder =>
                    builder.WithMisfireHandlingInstructionDoNothing())
                .ForJob(jobKey)
                .StartNow()
                .Build();
            // 添加新 Trigger（与旧 Trigger 共存）
            await scheduler.ScheduleJob(newTrigger, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to add trigger to job '{JobKey}' with cron '{Cron}'", jobKey, cron);
            return false;
        }
    }

    public async Task<bool> ReplaceTriggerAsync(JobKey jobKey, string cron, CancellationToken cancellationToken = default)
    {
        try
        {
            var oldTriggerKey = new TriggerKey(jobKey.Name, jobKey.Group);
            // 验证旧 Trigger 是否存在
            var oldTrigger = await scheduler.GetTrigger(oldTriggerKey, cancellationToken);
            if (oldTrigger == null)
                return false;
            var newTrigger = TriggerBuilder.Create()
               .WithIdentity(oldTriggerKey)
               .WithCronSchedule(cron, builder =>
                   builder.WithMisfireHandlingInstructionDoNothing())
                .ForJob(jobKey)
               .StartNow()
               .Build();
            // 关闭旧 Trigger 并添加新 Trigger
            await scheduler.UnscheduleJob(oldTriggerKey, cancellationToken);
            await scheduler.ScheduleJob(newTrigger, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to replace trigger for job '{JobKey}' with cron '{Cron}'", jobKey, cron);
            return false;
        }
    }

    public async Task<bool> CreateNewJobAsync(JobConfig jobConfig, Type? type, CancellationToken cancellationToken = default)
    {
        try
        {
            var jobKey = new JobKey(jobConfig.JobKeyName, jobConfig.Group);
            var triggerKey = new TriggerKey(jobConfig.TriggerKeyName, jobConfig.Group);
            // 创建 Trigger
            var trigger = TriggerBuilder.Create()
                .WithIdentity(triggerKey)
                .WithCronSchedule(jobConfig.Cron, builder =>
                    builder.WithMisfireHandlingInstructionDoNothing())
                .WithDescription(jobConfig.TriggerDescription)
                .StartNow()
                .Build();

            var jobBuilder = JobBuilder.Create()
                   .OfType(type)
                   .WithIdentity(jobKey)
                   .WithDescription(jobConfig.JobDescription);

            // 构建 JobDataMap（含重试/超时配置 + 用户自定义参数）
            var jobDataMap = new JobDataMap();
            if (jobConfig.MaxRetries > 0)
                jobDataMap.Put("MaxRetries", jobConfig.MaxRetries);
            if (jobConfig.RetryDelaySeconds > 0)
                jobDataMap.Put("RetryDelaySeconds", jobConfig.RetryDelaySeconds);
            if (jobConfig.TimeoutSeconds > 0)
                jobDataMap.Put("TimeoutSeconds", jobConfig.TimeoutSeconds);
            if (!string.IsNullOrWhiteSpace(jobConfig.JobData))
            {
                try
                {
                    var dict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(jobConfig.JobData);
                    if (dict != null)
                    {
                        foreach (var kvp in dict)
                            jobDataMap.Put(kvp.Key, kvp.Value);
                    }
                }
                catch (Newtonsoft.Json.JsonException) { }
            }
            jobBuilder.SetJobData(jobDataMap);

            var job = jobBuilder.Build();

            // 直接关联 Trigger 并添加到调度器
            await scheduler.ScheduleJob(job, trigger, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create new job '{JobKeyName}'", jobConfig.JobKeyName);
            return false;
        }
    }

    public async Task<List<TriggerRecord>> GetTriggerHistoryAsync(JobKey jobKey, int maxRecords = 100, CancellationToken cancellationToken = default)
    {
        return await store.GetRecordsAsync(jobKey, maxRecords, cancellationToken);
    }

    public async Task<DateTime> GetNextTriggerTimeAsync(JobKey jobKey, int maxRecords = 100, CancellationToken cancellationToken = default)
    {
        try
        {
            var res = await quartzContext.Triggers.FirstOrDefaultAsync(t => t.JobName == jobKey.Name, cancellationToken);
            if (res?.NextFireTime == null || res.NextFireTime <= 0)
                return DateTime.MinValue;
            return DateTimeOffset.FromUnixTimeMilliseconds(res.NextFireTime.Value).DateTime;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get next trigger time for job '{JobKey}'", jobKey);
            return DateTime.MinValue;
        }
    }
    #endregion

    #region Stats
    public async Task<JobStats?> GetJobStatsAsync(JobKey jobKey, CancellationToken cancellationToken = default)
    {
        try
        {
            var records = await store.GetRecordsAsync(jobKey, 1000, cancellationToken);
            if (records.Count == 0)
                return null;

            var trigger = quartzContext.Triggers
                .FirstOrDefault(t => t.JobName == jobKey.Name && t.JobGroup == jobKey.Group);

            var jobDetail = await scheduler.GetJobDetail(jobKey, cancellationToken);

            return new JobStats
            {
                JobKey = jobKey.Name,
                Group = jobKey.Group,
                TotalExecutions = records.Count,
                SuccessCount = records.Count(r => r.Success),
                FailureCount = records.Count(r => !r.Success),
                AvgDuration = records.Count > 0 ? Math.Round(records.Average(r => r.Duration), 1) : 0,
                LastFireTime = records.Max(r => r.FireTime),
                LastError = records.LastOrDefault(r => !r.Success)?.ErrorMessage,
                CronExpression = trigger?.GetType().GetProperty("CronExpression")?.GetValue(trigger)?.ToString(),
                NextFireTime = trigger?.NextFireTime.HasValue == true
                    ? DateTimeOffset.FromUnixTimeMilliseconds(trigger.NextFireTime.Value).LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss")
                    : null,
                Status = jobDetail != null ? "Active" : "Unknown"
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get stats for job '{JobKey}'", jobKey);
            return null;
        }
    }

    public async Task<List<JobStats>> GetAllJobStatsAsync(CancellationToken cancellationToken = default)
    {
        var stats = new List<JobStats>();
        try
        {
            var jobs = await GetAllJobsAsync(cancellationToken);
            if (jobs != null)
            {
                foreach (var job in jobs)
                {
                    var stat = await GetJobStatsAsync(job, cancellationToken);
                    if (stat != null)
                        stats.Add(stat);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get all job stats");
        }
        return stats;
    }
    #endregion
}
