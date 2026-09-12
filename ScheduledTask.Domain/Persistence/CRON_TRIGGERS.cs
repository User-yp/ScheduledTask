using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ScheduledTask.Domain.Persistence;

public class CRON_TRIGGERS
{
    [Key, Column(Order = 0)]
    public string SchedName { get; set; }

    [Key, Column(Order = 1)]
    public string TriggerName { get; set; }

    [Key, Column(Order = 2)]
    public string TriggerGroup { get; set; }

    public string CronExpression { get; set; }
    public string? TimeZoneId { get; set; }
    public CRON_TRIGGERS()
    {

    }

    public CRON_TRIGGERS(string schedName, string triggerName, string triggerGroup, string cronExpression, string? timeZoneId)
    {
        SchedName = schedName;
        TriggerName = triggerName;
        TriggerGroup = triggerGroup;
        CronExpression = cronExpression;
        TimeZoneId = timeZoneId;
    }
}
