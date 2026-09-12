namespace ScheduledTask;

public class QuartzOption
{
    public string SchedulerId { get; set; }
    public string SchedulerName { get; set; }
    public int MaxConcurrency { get; set; }
    public string TablePrefix { get; set; }
    public string MysqlConnStr { get; set; }
    public int CheckinInterval { get; set; }
    //redis链接
    public string ConnectionString { get; set; }

    //数据库
    public int DbNumber { get; set; }

    public QuartzOption() { }

    public QuartzOption(string schedulerId, string schedulerName, int maxConcurrency, string tablePrefix, string pgConnStr, int checkinInterval, string connectionString, int dbNumber)
    {
        SchedulerId = schedulerId;
        SchedulerName = schedulerName;
        MaxConcurrency = maxConcurrency;
        TablePrefix = tablePrefix;
        MysqlConnStr = pgConnStr;
        CheckinInterval = checkinInterval;
        ConnectionString = connectionString;
        DbNumber = dbNumber;
    }
}
