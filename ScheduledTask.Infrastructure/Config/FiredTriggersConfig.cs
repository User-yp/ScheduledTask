using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScheduledTask.Domain.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledTask.Infrastructure.Config;

public class FiredTriggersConfig : IEntityTypeConfiguration<FIRED_TRIGGERS>
{
    public void Configure(EntityTypeBuilder<FIRED_TRIGGERS> builder)
    {
        // 配置表名（保持与Quartz数据库一致）
        builder.ToTable($"qrtz_{nameof(FIRED_TRIGGERS).ToLower()}");

        // 配置复合主键（4个字段组成主键）
        builder.HasKey(f => new { f.SchedName, f.EntryId, f.TriggerName, f.TriggerGroup });

        // 配置各属性
        builder.Property(f => f.SchedName)
            .HasColumnName("sched_name")
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(f => f.EntryId)
            .HasColumnName("entry_id")
            .IsRequired()
            .HasMaxLength(95); // Quartz标准长度

        builder.Property(f => f.TriggerName)
            .HasColumnName("trigger_name")
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(f => f.TriggerGroup)
            .HasColumnName("trigger_group")
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(f => f.InstanceName)
            .HasColumnName("instance_name")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(f => f.FiredTime)
            .HasColumnName("fired_time")
            .IsRequired();

        builder.Property(f => f.SchedTime)
            .HasColumnName("sched_time")
            .IsRequired();

        builder.Property(f => f.Priority)
            .HasColumnName("priority")
            .IsRequired();

        builder.Property(f => f.State)
            .HasColumnName("state")
            .IsRequired()
            .HasMaxLength(16);

        builder.Property(f => f.JobName)
            .HasColumnName("job_name")
            .HasMaxLength(150);

        builder.Property(f => f.JobGroup)
            .HasColumnName("job_group")
            .HasMaxLength(150);

        builder.Property(f => f.IsNonConcurrent)
            .HasColumnName("is_nonconcurrent");

        builder.Property(f => f.RequestsRecovery)
            .HasColumnName("requests_recovery");
    }
}
