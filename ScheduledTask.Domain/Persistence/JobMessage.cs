using Quartz;
using ScheduledTask.Domain.IRepository;

namespace ScheduledTask.Domain.Persistence;

public class JobMessage<T> where T : IDefaultJob
{
    public string Group { get; set; }
    public string JobKeyName { get; set; }
    public string? JobDescription { get; set; }
    public string TriggerKeyName { get; set; }
    public string? TriggerDescription { get; set; }
    public string Cron { get; set; }
    public JobKey JobKey { get; set; }
    public TriggerKey TriggerKey { get; set; }
    public JobMessage() { }//提供无参构造方法给对象初始化器用
    public JobMessage(string group, string cron, string? jobDescription = null, string? triggerDescription = null)
    {
        Group = group;
        Cron = cron;
        JobDescription = jobDescription;
        TriggerDescription = triggerDescription;
        JobKeyName = typeof(T).Name;
        TriggerKeyName = typeof(T).Name;
        JobKey = new JobKey(JobKeyName, Group);
        TriggerKey = new TriggerKey(TriggerKeyName, Group);
    }
}
