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

public class CronTriggersConfig : IEntityTypeConfiguration<CRON_TRIGGERS>
{
    public void Configure(EntityTypeBuilder<CRON_TRIGGERS> builder)
    {
        // 配置表名（小写加下划线）
        builder.ToTable($"qrtz_{nameof(CRON_TRIGGERS).ToLower()}");

        // 配置复合主键（3个字段组成主键）
        builder.HasKey(c => new { c.SchedName, c.TriggerName, c.TriggerGroup });

        // 配置各属性（全部使用小写加下划线）
        builder.Property(c => c.SchedName)
            .HasColumnName("sched_name")
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(c => c.TriggerName)
            .HasColumnName("trigger_name")
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(c => c.TriggerGroup)
            .HasColumnName("trigger_group")
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(c => c.CronExpression)
            .HasColumnName("cron_expression")
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(c => c.TimeZoneId)
            .HasColumnName("time_zone_id")
            .HasMaxLength(80);
    }
}