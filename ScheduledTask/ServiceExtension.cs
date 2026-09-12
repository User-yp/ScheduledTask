using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Quartz;
using ScheduledTask.Domain.IRepository;
using ScheduledTask.Domain.Persistence;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Reflection;
using MySqlConnector;
using Quartz.Impl.AdoJobStore.Common;
using Quartz.Impl.AdoJobStore;

namespace ScheduledTask;

public static class ServiceExtension
{
    public static IServiceCollection InitService(this IServiceCollection services, IConfiguration configuration)
    {
        var option = configuration.GetSection(nameof(QuartzOption)).Get<QuartzOption>()
            ?? throw new ArgumentNullException(nameof(QuartzOption), "QuartzOption configuration is null or empty.");
        services.AddSingleton(option);

        services.AddDbContext<QuartzContext>(options =>
            options.UseMySql(
                option.MysqlConnStr,
                new MySqlServerVersion(new Version(8, 0, 21))
            ));
        // 注册 Quartz 服务  
        services.AddQuartz(quartz =>
        {
            quartz.SchedulerId = Environment.MachineName;
            quartz.SchedulerName = option.SchedulerName;
            quartz.UseSimpleTypeLoader();

            quartz.UsePersistentStore(store =>
            {
                store.UseNewtonsoftJsonSerializer();
                store.UseProperties = true;
                store.UseClustering(c => c.CheckinInterval = TimeSpan.FromSeconds(option.CheckinInterval));
                store.UseMySql(opt =>
                {
                    opt.UseDriverDelegate<MySQLDelegate>();
                    opt.ConnectionString = option.MysqlConnStr;
                    opt.TablePrefix = option.TablePrefix;
                });
            });
            quartz.UseDefaultThreadPool(tp =>
            {
                tp.MaxConcurrency = option.MaxConcurrency;
            });
        });
        services.AddAllJob();
        //services.AddSingleton(pro => pro.GetRequiredService<ISchedulerFactory>().GetScheduler().Result);
        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        return services;
    }
    public static async Task AddJobAsync(this IScheduler scheduler, IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        var opt = serviceProvider.GetRequiredService<QuartzOption>();
        // 创建临时连接
        var tempConnection = ConnectionMultiplexer.Connect(opt.ConnectionString);
        try
        {
            var db = tempConnection.GetDatabase(opt.DbNumber);
            var jogConfigs = db.HashGetAll(nameof(JobConfig)).ToConcurrentDictionary();

            var types = Assembly.GetEntryAssembly().GetTypes()
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
                else
                {
                    // 如果没有配置，则跳过
                    continue;
                }
            }
        }
        finally
        {
            await tempConnection.CloseAsync();
        }
    }
    public static IServiceCollection AddAllJob(this IServiceCollection services)
    {
        var types = Assembly.GetEntryAssembly().GetTypes()
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
                    var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(jobConfig.JobData);
                    if (dict != null)
                    {
                        foreach (var kvp in dict)
                            jobDataMap.Put(kvp.Key, kvp.Value);
                    }
                }
                catch (JsonException) { }
            }
            jobBuilder.SetJobData(jobDataMap);

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
    public static ConcurrentDictionary<string, string> ToConcurrentDictionary(this IEnumerable<HashEntry> entries)
    {
        var hashEntries = entries as HashEntry[] ?? entries.ToArray();
        if (hashEntries.Length == 0)
            return null;


        var dict = new ConcurrentDictionary<string, string>();
        foreach (var entry in hashEntries)
            dict[entry.Name] = entry.Value;

        return dict;
    }
}
