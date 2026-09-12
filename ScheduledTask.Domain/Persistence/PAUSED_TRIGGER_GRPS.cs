using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ScheduledTask.Domain.Persistence;

public class PAUSED_TRIGGER_GRPS
{
    [Key, Column(Order = 0)]
    public string SchedName { get; set; }

    [Key, Column(Order = 1)]
    public string TriggerGroup { get; set; }
    public PAUSED_TRIGGER_GRPS()
    {

    }

    public PAUSED_TRIGGER_GRPS(string schedName, string triggerGroup)
    {
        SchedName = schedName;
        TriggerGroup = triggerGroup;
    }
}
