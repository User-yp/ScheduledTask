using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ScheduledTask.Domain.Persistence;

public class CALENDARS
{
    [Key, Column(Order = 0)]
    public string SchedName { get; set; }

    [Key, Column(Order = 1)]
    public string CalendarName { get; set; }

    [Required]
    public byte[] Calendar { get; set; }
    public CALENDARS()
    {

    }

    public CALENDARS(string schedName, string calendarName, byte[] calendar)
    {
        SchedName = schedName;
        CalendarName = calendarName;
        Calendar = calendar;
    }
}
