using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ScheduledTask.Domain.Persistence;

public class BLOB_TRIGGERS
{
    [Key, Column(Order = 0)]
    public string SchedName { get; set; }

    [Key, Column(Order = 1)]
    public string TriggerName { get; set; }

    [Key, Column(Order = 2)]
    public string TriggerGroup { get; set; }

    public byte[]? BlobData { get; set; }
    public BLOB_TRIGGERS()
    {

    }

    public BLOB_TRIGGERS(string schedName, string triggerName, string triggerGroup, byte[]? blobData)
    {
        SchedName = schedName;
        TriggerName = triggerName;
        TriggerGroup = triggerGroup;
        BlobData = blobData;
    }
}
