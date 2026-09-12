using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Quartz;
using Quartz.Impl.AdoJobStore;
using Quartz.Impl.AdoJobStore.Common;
using Quartz.Impl.Matchers;
using ScheduledTask.Dll;
using ScheduledTask.Dll.Options;
using ScheduledTask.Domain.IRepository;
using ScheduledTask.Infrastructure.DbContexts;
using ScheduledTask.Infrastructure.Listener;
using ScheduledTask.Infrastructure.Notification;
using ScheduledTask.Infrastructure.Repository;
using System.Text;

namespace ScheduledTask.Infrastructure;

public static class ServiceExtension
{
    public static IServiceCollection InitService(this IServiceCollection services, IConfiguration configuration)
    {
        services.LoadConfiguration(configuration);
        //services.Configure<ConnectionOption>(configuration.GetSection(nameof(ConnectionOption)));



        services.AddDbContext<QuartzContext>((pro, opt) =>
        {
            var option = pro.GetRequiredService<QuartzOption>();
            opt.UseMySql(
                option.ConnStr,
                new MySqlServerVersion(new Version(8, 0, 21))
            );
        });


        services.RegistDllService();//configuration
        services.AddScoped<RegistService>();

        // 通知告警服务
        services.AddHttpClient("Webhook");
        services.AddSingleton<INotificationService, WebhookNotificationService>();

        services.AddQuartzAsync(configuration);
        return services;
    }
    public static IServiceCollection AddQuartzAsync(this IServiceCollection services, IConfiguration configuration)
    {
        var option = configuration.GetSection(nameof(QuartzOption)).Get<QuartzOption>();
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
                    opt.ConnectionString = option.ConnStr;
                    opt.TablePrefix = option.TablePrefix;
                });
            });
            quartz.UseDefaultThreadPool(tp =>
            {
                tp.MaxConcurrency = option.MaxConcurrency;
            });
            quartz.AddSchedulerListener<SchedulerListener>();
            quartz.AddJobListener<JobListener>(GroupMatcher<JobKey>.AnyGroup());
            quartz.AddTriggerListener<TriggerListener>(GroupMatcher<TriggerKey>.AnyGroup());
        });

        services.AddSingleton<ITriggerStore, DbTriggerStore>();
        services.AddAllJob();
        services.AddScoped<ISchedulerManager, SchedulerManager>();
        services.AddSingleton(pro => pro.GetRequiredService<ISchedulerFactory>().GetScheduler().Result);
        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        return services;
    }
}
