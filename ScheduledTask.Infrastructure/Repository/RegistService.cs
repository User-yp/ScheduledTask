using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Quartz;
using ScheduledTask.Dll.Redis;
using ScheduledTask.Domain.IRepository;
using ScheduledTask.Domain.Persistence;
using ScheduledTask.Infrastructure.DbContexts;
using System.Collections.Concurrent;
using System.Reflection;

namespace ScheduledTask.Infrastructure.Repository;

public class RegistService
{
    private readonly QuartzContext quartzContext;
    private readonly IRedisService redis;
    private readonly ISchedulerManager manager;

    public RegistService(IDbContextFactory<QuartzContext> contextFactory, IRedisService redis, ISchedulerManager manager)
    {
        quartzContext = contextFactory.CreateDbContext();
        this.redis = redis;
        this.manager = manager;
    }
    public async Task LoadConfigAsync()
    {
        // 从数据库加载配置到 Redis
        var jobConfigs = await quartzContext.JobConfigs
            .ToDictionaryAsync(cfg => cfg.JobKeyName, cfg => JsonConvert.SerializeObject(cfg));
        await redis.KeyDeleteAsync(nameof(JobConfig));
        await redis.HashSetAsync(nameof(JobConfig), new ConcurrentDictionary<string, string>(jobConfigs));
    }
    public async Task<bool> ReplaceTriggerAsync(string jobkey)
    {
        using var tx = await quartzContext.Database.BeginTransactionAsync();
        try
        {

            var jobJson = await redis.HashGetFieldAsync(nameof(JobConfig), jobkey);
            if (jobJson != null && jobJson.TryGetValue(jobkey, out var jobConfigJson))
            {
                var jobConfig = JsonConvert.DeserializeObject<JobConfig>(jobConfigJson);
                if (jobConfig == null)
                    return false;
                if (!await manager.DeleteJobAsync(new JobKey(jobConfig.JobKeyName, jobConfig.Group)))
                    return false;
                var type = Assembly.GetExecutingAssembly().GetTypes()
                    .FirstOrDefault(t => t.Name == jobConfig.JobKeyName && typeof(IDefaultJob).IsAssignableFrom(t));
                if (!await manager.CreateNewJobAsync(jobConfig, type))
                    return false;
                var dbJob = quartzContext.JobConfigs.FirstOrDefault(j => j.JobKeyName == jobConfig.JobKeyName);
                if (dbJob == null || string.IsNullOrEmpty(dbJob.JobKeyName))
                {
                    quartzContext.Add(jobConfig);
                }
                else
                {
                    dbJob.Cron = jobConfig.Cron;
                }
                await quartzContext.SaveChangesAsync();
                await tx.CommitAsync();
                return true;
            }
            return false;
        }
        catch (Exception)
        {
            await tx.RollbackAsync();
            return false;
        }

    }

    public async Task<bool> UpdateJobDataAsync(string jobkey, Dictionary<string, string> jobData, CancellationToken cancellationToken = default)
    {
        try
        {
            var dbJob = await quartzContext.JobConfigs
                .FirstOrDefaultAsync(j => j.JobKeyName == jobkey, cancellationToken);
            if (dbJob == null)
                return false;

            dbJob.JobData = JsonConvert.SerializeObject(jobData);
            await quartzContext.SaveChangesAsync(cancellationToken);

            // 同步到 Redis
            await redis.HashSetFieldAsync(nameof(JobConfig),
                new ConcurrentDictionary<string, string>(new Dictionary<string, string>
                {
                    [jobkey] = JsonConvert.SerializeObject(dbJob)
                }));

            // 热更新：自动重新调度作业使新 JobData 生效
            await RescheduleJobAsync(jobkey, cancellationToken);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 热更新：从 Redis 重新加载配置并重新调度作业
    /// </summary>
    public async Task<bool> RescheduleJobAsync(string jobkey, CancellationToken cancellationToken = default)
    {
        try
        {
            var jobJson = await redis.HashGetFieldAsync(nameof(JobConfig), jobkey);
            if (jobJson == null || !jobJson.TryGetValue(jobkey, out var jobConfigJson))
                return false;

            var jobConfig = JsonConvert.DeserializeObject<JobConfig>(jobConfigJson);
            if (jobConfig == null)
                return false;

            // 删除旧作业（含触发器）
            var oldJobKey = new JobKey(jobConfig.JobKeyName, jobConfig.Group);
            if (await manager.DeleteJobAsync(oldJobKey, cancellationToken))
            {
                await Task.Delay(200, cancellationToken); // 等待 Quartz 完成清理
            }

            // 查找作业类型并重新创建
            var type = Assembly.GetExecutingAssembly().GetTypes()
                .FirstOrDefault(t => t.Name == jobConfig.JobKeyName && typeof(IDefaultJob).IsAssignableFrom(t));
            if (type == null)
                return false;

            return await manager.CreateNewJobAsync(jobConfig, type, cancellationToken);
        }
        catch (Exception)
        {
            return false;
        }
    }
}
