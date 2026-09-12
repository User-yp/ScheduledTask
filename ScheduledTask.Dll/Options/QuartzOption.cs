namespace ScheduledTask.Dll.Options;

[Option]
public class QuartzOption
{
    public string SchedulerId { get; set; }
    public string SchedulerName { get; set; }
    public int MaxConcurrency { get; set; }
    public string TablePrefix { get; set; }
    public string ConnStr { get; set; }
    public int CheckinInterval { get; set; }
    public QuartzOption() { }
    public QuartzOption(string schedulerId, string schedulerName, int maxConcurrency, string tablePrefix, string ConnStr, int checkinInterval)
    {
        SchedulerId = schedulerId;
        SchedulerName = schedulerName;
        MaxConcurrency = maxConcurrency;
        TablePrefix = tablePrefix;
        ConnStr = ConnStr;
        CheckinInterval = checkinInterval;
    }
}
