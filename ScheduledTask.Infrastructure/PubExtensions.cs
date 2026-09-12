using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Quartz;
using ScheduledTask.Dll.Redis;
using ScheduledTask.Domain.IRepository;
using ScheduledTask.Domain.Persistence;
using ScheduledTask.Infrastructure;
using System.Data;
using System.Reflection;

namespace ScheduledTask.Infrastructure;

public static class PubExtensions
{

    public static async Task AddJobAsync(this IScheduler scheduler, IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        var redis = serviceProvider.GetRequiredService<IRedisService>();
        var jogConfigs = await redis.HashGetAsync(nameof(JobConfig));

        var types = Assembly.GetExecutingAssembly().GetTypes()
        .Where(t => !t.IsAbstract && typeof(IDefaultJob).IsAssignableFrom(t));

        foreach (var type in types)
        {
            if (jogConfigs.TryGetValue(type.Name, out var jogConfig))
            {
                var config = JsonConvert.DeserializeObject<JobConfig>(jogConfig) ??
                    throw new ArgumentNullException(nameof(jogConfig), $"JobConfig for {type.Name} is null or empty.");
                // 检查是否启用
                if (!config.IsEnabled)
                    continue;
                // 检查 Cron 表达式是否有效
                if (!ValidateCron(config.Cron))
                    continue;
                // 创建默认 Job
                await scheduler.CreateDefaultJobAsync(config, type, cancellationToken);
            }
        }
    }
    public static IServiceCollection AddAllJob(this IServiceCollection services)
    {
        var types = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => !t.IsAbstract && typeof(IDefaultJob).IsAssignableFrom(t));
        foreach (var type in types)
        {
            services.AddTransient(type);
        }
        return services;
    }
    public static async Task CreateDefaultJobAsync(this IScheduler scheduler, JobConfig jobConfig, Type type, CancellationToken cancellationToken = default)
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

        var jobExists = await scheduler.CheckExists(jobKey, cancellationToken);
        if (!jobExists)
        {
            var jobBuilder = JobBuilder.Create()
                .OfType(type)
                .WithIdentity(jobKey)
                .WithDescription(jobConfig.JobDescription);

            // 支持 JobDataMap 动态参数 + 重试/超时配置
            ApplyJobData(jobBuilder, jobConfig);

            var job = jobBuilder.Build();

            // 直接关联 Trigger 并添加到调度器
            await scheduler.ScheduleJob(job, trigger, cancellationToken);
        }
        else
        {
            // 若 Job 已存在，直接关联 Trigger
            await scheduler.ScheduleJob(trigger, cancellationToken);
        }
    }
    /// <summary>
    /// 从 JobConfig 解析并应用 JobDataMap 到 JobBuilder（含重试/超时配置）
    /// </summary>
    private static void ApplyJobData(JobBuilder jobBuilder, JobConfig jobConfig)
    {
        var jobDataMap = new JobDataMap();

        // 注入重试/超时配置
        if (jobConfig.MaxRetries > 0)
            jobDataMap.Put("MaxRetries", jobConfig.MaxRetries);
        if (jobConfig.RetryDelaySeconds > 0)
            jobDataMap.Put("RetryDelaySeconds", jobConfig.RetryDelaySeconds);
        if (jobConfig.TimeoutSeconds > 0)
            jobDataMap.Put("TimeoutSeconds", jobConfig.TimeoutSeconds);

        // 解析用户自定义 JobData
        if (!string.IsNullOrWhiteSpace(jobConfig.JobData))
        {
            try
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(jobConfig.JobData);
                if (dict != null)
                {
                    foreach (var kvp in dict)
                    {
                        jobDataMap.Put(kvp.Key, kvp.Value);
                    }
                }
            }
            catch (JsonException)
            {
                // JobData 格式无效，跳过
            }
        }

        jobBuilder.SetJobData(jobDataMap);
    }

    public static bool ValidateCron(this string cron)
    {
        if (string.IsNullOrEmpty(cron))
            return false;
        try
        {
            new CronExpression(cron);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
