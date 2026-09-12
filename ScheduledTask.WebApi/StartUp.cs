using Quartz;
using ScheduledTask.Infrastructure.Repository;
using ScheduledTask.Infrastructure;

public static class StartUp
{
    public static async Task StartUpAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var regist = scope.ServiceProvider.GetRequiredService<RegistService>();
        var scheduler = scope.ServiceProvider.GetRequiredService<IScheduler>();
        try
        {
            if (!scheduler.IsStarted)
            {
                //每次启动时加载配置
                await regist.LoadConfigAsync();
                await scheduler.AddJobAsync(scope.ServiceProvider);
                await scheduler.StartDelayed(TimeSpan.FromMilliseconds(500));
            }
        }
        catch (Exception ex)
        {
            var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("StartUp");
            logger.LogError(ex, "Failed to start scheduler");
            throw;
        }
    }
}