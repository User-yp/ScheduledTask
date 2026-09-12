using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ScheduledTask.Domain.Persistence;

public class TRIGGERS
{
    [Key, Column(Order = 0)]
    public string SchedName { get; set; }

    [Key, Column(Order = 1)]
    public string TriggerName { get; set; }

    [Key, Column(Order = 2)]
    public string TriggerGroup { get; set; }

    public string JobName { get; set; }
    public string JobGroup { get; set; }
    public string? Description { get; set; }
    public long? NextFireTime { get; set; }
    public long? PrevFireTime { get; set; }
    public int? Priority { get; set; }
    public string TriggerState { get; set; }
    public string TriggerType { get; set; }
    public long StartTime { get; set; }
    public long? EndTime { get; set; }
    public string? CalendarName { get; set; }
    public short? MisfireInstr { get; set; }
    public byte[]? JobData { get; set; }

    [ForeignKey("SchedName,JobName,JobGroup")]
    public virtual JOB_DETAILS JobDetail { get; set; }
    public TRIGGERS()
    {

    }

    public TRIGGERS(string schedName, string triggerName, string triggerGroup, string jobName, string jobGroup,
        string? description, long? nextFireTime, long? prevFireTime, int? priority, string triggerState, string triggerType,
        long startTime, long? endTime, string? calendarName, short? misfireInstr, byte[]? jobData, JOB_DETAILS jobDetail)
    {
        SchedName = schedName;
        TriggerName = triggerName;
        TriggerGroup = triggerGroup;
        JobName = jobName;
        JobGroup = jobGroup;
        Description = description;
        NextFireTime = nextFireTime;
        PrevFireTime = prevFireTime;
        Priority = priority;
        TriggerState = triggerState;
        TriggerType = triggerType;
        StartTime = startTime;
        EndTime = endTime;
        CalendarName = calendarName;
        MisfireInstr = misfireInstr;
        JobData = jobData;
        JobDetail = jobDetail;
    }
}
