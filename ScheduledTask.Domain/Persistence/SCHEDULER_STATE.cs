using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ScheduledTask.Domain.Persistence;

public class SCHEDULER_STATE
{
    [Key, Column(Order = 0)]
    public string SchedName { get; set; }

    [Key, Column(Order = 1)]
    public string InstanceName { get; set; }

    public long LastCheckinTime { get; set; }
    public long CheckinInterval { get; set; }
    public SCHEDULER_STATE()
    {

    }

    public SCHEDULER_STATE(string schedName, string instanceName, long lastCheckinTime, long checkinInterval)
    {
        SchedName = schedName;
        InstanceName = instanceName;
        LastCheckinTime = lastCheckinTime;
        CheckinInterval = checkinInterval;
    }
}
