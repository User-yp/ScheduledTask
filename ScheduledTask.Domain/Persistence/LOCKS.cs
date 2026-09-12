using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ScheduledTask.Domain.Persistence;

public class LOCKS
{
    [Key, Column(Order = 0)]
    public string SchedName { get; set; }

    [Key, Column(Order = 1)]
    public string LockName { get; set; }
    public LOCKS()
    {

    }

    public LOCKS(string schedName, string lockName)
    {
        SchedName = schedName;
        LockName = lockName;
    }
}
