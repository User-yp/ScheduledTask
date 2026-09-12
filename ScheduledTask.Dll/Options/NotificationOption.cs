namespace ScheduledTask.Dll.Options;

/// <summary>
/// 通知告警配置
/// </summary>
[Option]
public class NotificationOption
{
    /// <summary>
    /// 是否启用告警通知
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Webhook URL（支持钉钉/飞书/企业微信）
    /// </summary>
    public string WebhookUrl { get; set; } = string.Empty;

    /// <summary>
    /// 通知平台类型: dingtalk / feishu / wecom
    /// </summary>
    public string Platform { get; set; } = "dingtalk";
}
