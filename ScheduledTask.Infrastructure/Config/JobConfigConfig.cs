using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScheduledTask.Domain.Persistence;

namespace ScheduledTask.Infrastructure.Config;

internal class JobConfigConfig : IEntityTypeConfiguration<JobConfig>
{
    public void Configure(EntityTypeBuilder<JobConfig> builder)
    {
        builder.ToTable("JOB_CONFIG");
        builder.HasKey(j => new { j.Group, j.JobKeyName });
        builder.HasIndex(p => p.Group);
        builder.HasIndex(p => p.JobKeyName);

        builder.Property(j => j.Group)
            .HasColumnName("GROUP")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(j => j.JobKeyName)
            .HasColumnName("JOB_KEYNAME")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(j => j.JobDescription)
            .HasColumnName("JOB_DESC")
            .HasMaxLength(100);

        builder.Property(j => j.TriggerKeyName)
            .HasColumnName("TRIGGER_KEYNAME")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(j => j.TriggerDescription)

        .HasColumnName("TRIGGER_DESC")
            .HasMaxLength(100);

        builder.Property(j => j.Cron)
            .HasColumnName("CRON")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(j => j.CronDescription)
            .HasColumnName("CRON_DESC")
            .HasMaxLength(100);

        builder.Property(j => j.JobData)
            .HasColumnName("JOB_DATA")
            .HasColumnType("longtext");

        builder.Property(j => j.MaxRetries)
            .HasColumnName("MAX_RETRIES")
            .HasDefaultValue(0);

        builder.Property(j => j.RetryDelaySeconds)
            .HasColumnName("RETRY_DELAY")
            .HasDefaultValue(60);

        builder.Property(j => j.TimeoutSeconds)
            .HasColumnName("TIMEOUT_SECONDS")
            .HasDefaultValue(0);

        builder.Property(j => j.IsEnable)
            .HasColumnName("IS_ENABLE")
            .IsRequired()
            .HasMaxLength(1)
            .HasDefaultValue("N"); // 默认值为 "N" 表示未启用
    }
}
