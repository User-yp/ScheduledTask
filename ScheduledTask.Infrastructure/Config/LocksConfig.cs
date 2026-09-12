using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Org.BouncyCastle.Utilities;
using ScheduledTask.Domain.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledTask.Infrastructure.Config;

public class LocksConfig : IEntityTypeConfiguration<LOCKS>
{
    public void Configure(EntityTypeBuilder<LOCKS> builder)
    {
        // 配置表名（保持与Quartz数据库一致）
        builder.ToTable($"qrtz_{nameof(LOCKS).ToLower()}");

        // 配置复合主键
        builder.HasKey(l => new { l.SchedName, l.LockName });

        // 配置各属性
        builder.Property(l => l.SchedName)
            .HasColumnName("sched_name")
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(l => l.LockName)
            .HasColumnName("lock_name")
            .IsRequired()
            .HasMaxLength(40);
    }
}
