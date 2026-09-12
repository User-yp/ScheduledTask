using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ScheduledTask.Domain.Persistence;

public class SIMPROP_TRIGGERS
{
    [Key, Column(Order = 0)]
    public string SchedName { get; set; }

    [Key, Column(Order = 1)]
    public string TriggerName { get; set; }

    [Key, Column(Order = 2)]
    public string TriggerGroup { get; set; }

    public string? StrProp1 { get; set; }
    public string? StrProp2 { get; set; }
    public string? StrProp3 { get; set; }
    public int? IntProp1 { get; set; }
    public int? IntProp2 { get; set; }
    public long? LongProp1 { get; set; }
    public long? LongProp2 { get; set; }
    public decimal? DecProp1 { get; set; }
    public decimal? DecProp2 { get; set; }
    public bool? BoolProp1 { get; set; }
    public bool? BoolProp2 { get; set; }
    public string? TimeZoneId { get; set; }
    public SIMPROP_TRIGGERS()
    {

    }

    public SIMPROP_TRIGGERS(string schedName, string triggerName, string triggerGroup, string? strProp1, string? strProp2, string? strProp3, int? intProp1, int? intProp2, long? longProp1, long? longProp2, decimal? decProp1, decimal? decProp2, bool? boolProp1, bool? boolProp2, string? timeZoneId)
    {
        SchedName = schedName;
        TriggerName = triggerName;
        TriggerGroup = triggerGroup;
        StrProp1 = strProp1;
        StrProp2 = strProp2;
        StrProp3 = strProp3;
        IntProp1 = intProp1;
        IntProp2 = intProp2;
        LongProp1 = longProp1;
        LongProp2 = longProp2;
        DecProp1 = decProp1;
        DecProp2 = decProp2;
        BoolProp1 = boolProp1;
        BoolProp2 = boolProp2;
        TimeZoneId = timeZoneId;
    }
}
