using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using ScheduledTask;

// 创建主机
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.InitService(context.Configuration);
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    })
    .Build();
using var scope = host.Services.CreateScope();
var scheduler = scope.ServiceProvider.GetRequiredService<ISchedulerFactory>().GetScheduler().Result;
try
{
    if (!scheduler.IsStarted)
    {
        await scheduler.AddJobAsync(scope.ServiceProvider);
        await scheduler.StartDelayed(TimeSpan.FromMilliseconds(500));
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

// 运行主机（如果有后台服务）
await host.RunAsync();
