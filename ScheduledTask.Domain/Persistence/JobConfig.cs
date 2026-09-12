using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduledTask.Domain.Persistence;

public class JobConfig
{
    public string Group { get; set; }
    public string JobKeyName { get; set; }
    public string? JobDescription { get; set; }
    public string TriggerKeyName { get; set; }
    public string? TriggerDescription { get; set; }
    public string Cron { get; set; }
    public string? CronDescription { get; set; }
    /// <summary>
    /// 作业动态参数 (JSON 格式): {"key1":"value1","key2":"value2"}
    /// </summary>
    public string? JobData { get; set; }
    /// <summary>
    /// 失败重试次数 (0=不重试)
    /// </summary>
    public int MaxRetries { get; set; }
    /// <summary>
    /// 重试间隔 (秒)
    /// </summary>
    public int RetryDelaySeconds { get; set; } = 60;
    /// <summary>
    /// 作业执行超时时间 (秒, 0=不限制)
    /// </summary>
    public int TimeoutSeconds { get; set; }
    /// <summary>
    /// 是否启用: "Y"=启用, "N"=禁用
    /// </summary>
    public string IsEnable { get; set; } = "Y";

    /// <summary>
    /// 计算属性: 作业是否已启用
    /// </summary>
    [NotMapped]
    public bool IsEnabled => !string.Equals(IsEnable, "N", StringComparison.OrdinalIgnoreCase);

    public JobConfig() { }
    public JobConfig(string group, string jobKeyName, string? jobDescription, string triggerKeyName,
        string? triggerDescription, string cron, string isEnabled, string? cronDescription)
    {
        Group = group;
        JobKeyName = jobKeyName;
        JobDescription = jobDescription;
        TriggerKeyName = triggerKeyName;
        TriggerDescription = triggerDescription;
        Cron = cron;
        IsEnable = isEnabled;
        CronDescription = cronDescription;
    }
}
