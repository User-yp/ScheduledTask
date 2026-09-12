using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScheduledTask.Domain.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledTask.Infrastructure.Config;

public class SimpropTriggersConfig : IEntityTypeConfiguration<SIMPROP_TRIGGERS>
{
    public void Configure(EntityTypeBuilder<SIMPROP_TRIGGERS> builder)
    {
        // 配置表名（小写加下划线）
        builder.ToTable($"qrtz_{nameof(SIMPROP_TRIGGERS).ToLower()}");

        // 配置复合主键
        builder.HasKey(s => new { s.SchedName, s.TriggerName, s.TriggerGroup });

        // 配置主键属性
        builder.Property(s => s.SchedName)
            .HasColumnName("sched_name")
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(s => s.TriggerName)
            .HasColumnName("trigger_name")
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(s => s.TriggerGroup)
            .HasColumnName("trigger_group")
            .IsRequired()
            .HasMaxLength(150);

        // 配置字符串属性
        builder.Property(s => s.StrProp1)
            .HasColumnName("str_prop_1")
            .HasMaxLength(512);

        builder.Property(s => s.StrProp2)
            .HasColumnName("str_prop_2")
            .HasMaxLength(512);

        builder.Property(s => s.StrProp3)
            .HasColumnName("str_prop_3")
            .HasMaxLength(512);

        // 配置数值属性
        builder.Property(s => s.IntProp1)
            .HasColumnName("int_prop_1");

        builder.Property(s => s.IntProp2)
            .HasColumnName("int_prop_2");

        builder.Property(s => s.LongProp1)
            .HasColumnName("long_prop_1");

        builder.Property(s => s.LongProp2)
            .HasColumnName("long_prop_2");

        // 配置十进制属性
        builder.Property(s => s.DecProp1)
            .HasColumnName("dec_prop_1")
            .HasColumnType("numeric(13,4)");  // 根据实际需求调整精度

        builder.Property(s => s.DecProp2)
            .HasColumnName("dec_prop_2")
            .HasColumnType("numeric(13,4)");

        // 配置布尔属性
        builder.Property(s => s.BoolProp1)
            .HasColumnName("bool_prop_1");

        builder.Property(s => s.BoolProp2)
            .HasColumnName("bool_prop_2");

        // 配置时区属性
        builder.Property(s => s.TimeZoneId)
            .HasColumnName("time_zone_id")
            .HasMaxLength(80);
    }
}
