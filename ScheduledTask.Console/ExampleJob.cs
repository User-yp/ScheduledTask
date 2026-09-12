using Quartz;
using ScheduledTask.Domain.IRepository;

namespace ScheduledTask.Console;

public class ExampleJob : IDefaultJob
{
    public Task Execute(IJobExecutionContext context)
    {
        System.Console.WriteLine($"自动触发---{DateTime.Now}");
        return Task.CompletedTask;
    }
}
