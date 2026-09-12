using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScheduledTask.Domain.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledTask.Infrastructure.Config;

public class PausedTriggerGroupsConfig : IEntityTypeConfiguration<PAUSED_TRIGGER_GRPS>
{
    public void Configure(EntityTypeBuilder<PAUSED_TRIGGER_GRPS> builder)
    {
        // 配置表名（小写加下划线）
        builder.ToTable($"qrtz_{nameof(PAUSED_TRIGGER_GRPS).ToLower()}");

        // 配置复合主键
        builder.HasKey(p => new { p.SchedName, p.TriggerGroup });

        // 配置各属性（小写加下划线）
        builder.Property(p => p.SchedName)
            .HasColumnName("sched_name")
            .IsRequired()
            .HasMaxLength(120);  // 根据Quartz标准长度设置

        builder.Property(p => p.TriggerGroup)
            .HasColumnName("trigger_group")
            .IsRequired()
            .HasMaxLength(150);  // Quartz标准长度
    }
}
