using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ScheduledTask.Dll.Options;
using ScheduledTask.Domain.IRepository;
using System.Net.Http;
using System.Text;

namespace ScheduledTask.Infrastructure.Notification;

public class WebhookNotificationService : INotificationService
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ILogger<WebhookNotificationService> logger;
    private readonly NotificationOption? option;

    public WebhookNotificationService(
        IHttpClientFactory httpClientFactory,
        ILogger<WebhookNotificationService> logger,
        IServiceProvider serviceProvider)
    {
        this.httpClientFactory = httpClientFactory;
        this.logger = logger;
        // 尝试从 DI 获取配置（可能从 Redis 或 appsettings 加载）
        this.option = serviceProvider.GetService<NotificationOption>();
    }

    public async Task SendFailureNotificationAsync(NotificationMessage message, CancellationToken cancellationToken = default)
    {
        if (option == null || !option.Enabled || string.IsNullOrWhiteSpace(option.WebhookUrl))
        {
            logger.LogDebug("Notification skipped: not configured or disabled");
            return;
        }

        try
        {
            var payload = BuildPayload(message, option.Platform);
            var json = JsonConvert.SerializeObject(payload);

            var client = httpClientFactory.CreateClient("Webhook");
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(option.WebhookUrl, content, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("Failure notification sent for job {JobKey}", message.JobKey);
            }
            else
            {
                logger.LogWarning("Notification webhook returned {StatusCode} for job {JobKey}",
                    (int)response.StatusCode, message.JobKey);
            }
        }
        catch (Exception ex)
        {
            // 通知失败不应影响主流程
            logger.LogWarning(ex, "Failed to send failure notification for job {JobKey}", message.JobKey);
        }
    }

    private static object BuildPayload(NotificationMessage msg, string platform)
    {
        var title = $"⚠️ 任务执行失败 - {msg.JobKey}";
        var text = $"### {title}\n\n" +
                   $"- **任务名称**: {msg.JobKey}\n" +
                   $"- **触发器**: {msg.TriggerKey}\n" +
                   $"- **触发时间**: {msg.FireTime}\n" +
                   $"- **执行耗时**: {msg.Duration}ms\n" +
                   $"- **错误信息**: {msg.ErrorMessage ?? "无"}\n" +
                   $"- **调度器**: {msg.SchedulerName} ({msg.SchedulerId})\n";

        return platform.ToLower() switch
        {
            "feishu" => new { msg_type = "interactive", content = new { title, text } },
            "wecom" => new { msgtype = "markdown", markdown = new { content = text } },
            _ => new { msgtype = "markdown", markdown = new { title, text } } // dingtalk default
        };
    }
}
