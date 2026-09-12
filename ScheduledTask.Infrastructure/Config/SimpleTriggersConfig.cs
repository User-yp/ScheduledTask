using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScheduledTask.Domain.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Quartz.MisfireInstruction;
namespace ScheduledTask.Infrastructure.Config;

public class SimpleTriggersConfig : IEntityTypeConfiguration<SIMPLE_TRIGGERS>
{
    public void Configure(EntityTypeBuilder<SIMPLE_TRIGGERS> builder)
    {
        // 配置表名（小写加下划线
        builder.ToTable($"qrtz_{nameof(SIMPLE_TRIGGERS).ToLower()}");

        // 配置复合主键（3个字段组成主键）
        builder.HasKey(s => new { s.SchedName, s.TriggerName, s.TriggerGroup });

        // 配置各属性（小写加下划线）
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

        builder.Property(s => s.RepeatCount)
            .HasColumnName("repeat_count")
            .IsRequired();

        builder.Property(s => s.RepeatInterval)
            .HasColumnName("repeat_interval")
            .IsRequired();

        builder.Property(s => s.TimesTriggered)
            .HasColumnName("times_triggered")
            .IsRequired();
    }
}
