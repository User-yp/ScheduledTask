using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScheduledTask.Domain.Persistence;

namespace ScheduledTask.Infrastructure.Config;

internal class BlobTriggersConfig : IEntityTypeConfiguration<BLOB_TRIGGERS>
{
    public void Configure(EntityTypeBuilder<BLOB_TRIGGERS> builder)
    {
        builder.ToTable($"qrtz_{nameof(BLOB_TRIGGERS).ToLower()}");
        // 配置复合主键
        builder.HasKey(t => new { t.SchedName, t.TriggerName, t.TriggerGroup });

        // 配置各属性
        builder.Property(t => t.SchedName)
            .HasColumnName("SCHED_NAME")
            .IsRequired()
            .HasMaxLength(120); // 根据实际数据库设置长度

        builder.Property(t => t.TriggerName)
            .HasColumnName("TRIGGER_NAME")
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(t => t.TriggerGroup)
            .HasColumnName("TRIGGER_GROUP")
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(t => t.BlobData)
            .HasColumnName("BLOB_DATA");
    }
}
