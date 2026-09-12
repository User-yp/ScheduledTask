using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ScheduledTask.Domain.Persistence;

public class SIMPLE_TRIGGERS
{
    [Key, Column(Order = 0)]
    public string SchedName { get; set; }

    [Key, Column(Order = 1)]
    public string TriggerName { get; set; }

    [Key, Column(Order = 2)]
    public string TriggerGroup { get; set; }

    public long RepeatCount { get; set; }
    public long RepeatInterval { get; set; }
    public long TimesTriggered { get; set; }
    public SIMPLE_TRIGGERS()
    {

    }

    public SIMPLE_TRIGGERS(string schedName, string triggerName, string triggerGroup, long repeatCount, long repeatInterval, long timesTriggered)
    {
        SchedName = schedName;
        TriggerName = triggerName;
        TriggerGroup = triggerGroup;
        RepeatCount = repeatCount;
        RepeatInterval = repeatInterval;
        TimesTriggered = timesTriggered;
    }
}
