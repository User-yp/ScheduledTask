using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScheduledTask.Domain.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledTask.Infrastructure.Config;

public class SchedulerStateConfig : IEntityTypeConfiguration<SCHEDULER_STATE>
{
    public void Configure(EntityTypeBuilder<SCHEDULER_STATE> builder)
    {
        // 配置表名（保持与数据库一致）
        builder.ToTable($"qrtz_{nameof(SCHEDULER_STATE).ToLower()}");

        // 配置复合主键
        builder.HasKey(s => new { s.SchedName, s.InstanceName });

        // 配置各属性（明确指定列名和约束）
        builder.Property(s => s.SchedName)
            .HasColumnName("sched_name")
            .IsRequired()
            .HasMaxLength(120); // 根据实际数据库调整长度

        builder.Property(s => s.InstanceName)
            .HasColumnName("instance_name")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.LastCheckinTime)
            .HasColumnName("last_checkin_time")
            .IsRequired();

        builder.Property(s => s.CheckinInterval)
            .HasColumnName("checkin_interval")
            .IsRequired();
    }
}
