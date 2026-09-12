namespace ScheduledTask.Domain.Persistence;

public class TriggerRecord
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string SchedulerName { get; set; }
    public string SchedulerId { get; set; }
    /// <summary>
    /// 触发时间 (UTC)
    /// </summary>
    public DateTime FireTime { get; set; }
    /// <summary>
    /// 触发器标识
    /// </summary>
    public string TriggerKey { get; set; }
    /// <summary>
    /// 任务标识
    /// </summary>
    public string JobKey { get; set; }
    /// <summary>
    /// 执行耗时 (毫秒)
    /// </summary>
    public int Duration { get; set; }
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }
    /// <summary>
    /// 错误信息（可选）
    /// </summary>
    public string? ErrorMessage { get; set; }

    public TriggerRecord()
    {
    }

    public TriggerRecord(DateTime fireTime, string jobKey, int duration, bool success, string? errorMessage, string schedulerName, string schedulerId)
    {
        FireTime = fireTime;
        JobKey = jobKey;
        Duration = duration;
        Success = success;
        ErrorMessage = errorMessage;
        SchedulerName = schedulerName;
        SchedulerId = schedulerId;
    }
}
