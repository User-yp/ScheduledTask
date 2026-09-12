using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace ScheduledTask.Domain.Persistence;

public class JOB_DETAILS
{
    [Key, Column(Order = 0)]
    public string SchedName { get; set; }

    [Key, Column(Order = 1)]
    public string JobName { get; set; }

    [Key, Column(Order = 2)]
    public string JobGroup { get; set; }

    public string? Description { get; set; }
    public string JobClassName { get; set; }
    public bool IsDurable { get; set; }
    public bool IsNonConcurrent { get; set; }
    public bool IsUpdateData { get; set; }
    public bool RequestsRecovery { get; set; }
    public byte[]? JobData { get; set; }
    public virtual ICollection<TRIGGERS> Triggers { get; set; } = new List<TRIGGERS>();
    public JOB_DETAILS()
    {

    }

    public JOB_DETAILS(string schedName, string jobName, string jobGroup, string? description, string jobClassName, bool isDurable, bool isNonConcurrent, bool isUpdateData, bool requestsRecovery, byte[]? jobData, ICollection<TRIGGERS> triggers)
    {
        SchedName = schedName;
        JobName = jobName;
        JobGroup = jobGroup;
        Description = description;
        JobClassName = jobClassName;
        IsDurable = isDurable;
        IsNonConcurrent = isNonConcurrent;
        IsUpdateData = isUpdateData;
        RequestsRecovery = requestsRecovery;
        JobData = jobData;
        Triggers = triggers;
    }
}
