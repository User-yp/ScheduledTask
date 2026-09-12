using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScheduledTask.Domain.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledTask.Infrastructure.Config;

public class CalendarsConfig : IEntityTypeConfiguration<CALENDARS>
{
    public void Configure(EntityTypeBuilder<CALENDARS> builder)
    {
        // 配置表名（小写加下划线）
        builder.ToTable($"qrtz_{nameof(CALENDARS).ToLower()}");

        // 配置复合主键
        builder.HasKey(c => new { c.SchedName, c.CalendarName });

        // 配置各属性（小写加下划线）
        builder.Property(c => c.SchedName)
            .HasColumnName("sched_name")
            .IsRequired()
            .HasMaxLength(120);  // 根据Quartz标准长度设置

        builder.Property(c => c.CalendarName)
            .HasColumnName("calendar_name")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Calendar)
            .HasColumnName("calendar")
            .IsRequired()
            .HasColumnType("longblob");
    }
}
