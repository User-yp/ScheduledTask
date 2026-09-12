using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ScheduledTask.Domain.Persistence;

public class FIRED_TRIGGERS
{
    [Key, Column(Order = 0)]
    public string SchedName { get; set; }

    [Key, Column(Order = 1)]
    public string EntryId { get; set; }

    [Key, Column(Order = 2)]
    public string TriggerName { get; set; }

    [Key, Column(Order = 3)]
    public string TriggerGroup { get; set; }

    public string InstanceName { get; set; }
    public long FiredTime { get; set; }
    public long SchedTime { get; set; }
    public int Priority { get; set; }
    public string State { get; set; }
    public string? JobName { get; set; }
    public string? JobGroup { get; set; }
    public bool? IsNonConcurrent { get; set; }
    public bool? RequestsRecovery { get; set; }
    public FIRED_TRIGGERS()
    {

    }

    public FIRED_TRIGGERS(string schedName, string entryId, string triggerName, string triggerGroup, string instanceName, long firedTime, long schedTime, int priority, string state, string? jobName, string? jobGroup, bool? isNonConcurrent, bool? requestsRecovery)
    {
        SchedName = schedName;
        EntryId = entryId;
        TriggerName = triggerName;
        TriggerGroup = triggerGroup;
        InstanceName = instanceName;
        FiredTime = firedTime;
        SchedTime = schedTime;
        Priority = priority;
        State = state;
        JobName = jobName;
        JobGroup = jobGroup;
        IsNonConcurrent = isNonConcurrent;
        RequestsRecovery = requestsRecovery;
    }
}
