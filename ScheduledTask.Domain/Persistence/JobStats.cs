namespace ScheduledTask.Domain.Persistence;

/// <summary>
/// 作业执行统计
/// </summary>
public class JobStats
{
    public string JobKey { get; set; }
    public string Group { get; set; }
    public int TotalExecutions { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public double SuccessRate => TotalExecutions > 0 ? Math.Round((double)SuccessCount / TotalExecutions * 100, 1) : 0;
    public double AvgDuration { get; set; }
    public DateTime? LastFireTime { get; set; }
    public string? LastError { get; set; }
    public string? CronExpression { get; set; }
    public string? NextFireTime { get; set; }
    public string Status { get; set; } = "Unknown"; // Running / Paused / Unknown
}
