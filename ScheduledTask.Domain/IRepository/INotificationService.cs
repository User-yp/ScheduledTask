namespace ScheduledTask.Domain.IRepository;

/// <summary>
/// 告警通知消息
/// </summary>
public class NotificationMessage
{
    public string JobKey { get; set; }
    public string TriggerKey { get; set; }
    public string FireTime { get; set; }
    public int Duration { get; set; }
    public string? ErrorMessage { get; set; }
    public string SchedulerName { get; set; }
    public string SchedulerId { get; set; }
}

/// <summary>
/// 通知服务接口（支持钉钉/飞书/企业微信 Webhook）
/// </summary>
public interface INotificationService
{
    Task SendFailureNotificationAsync(NotificationMessage message, CancellationToken cancellationToken = default);
}
