using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScheduledTask.Domain.Persistence;
using System.Threading.Tasks;

namespace ScheduledTask.Infrastructure.Config;

public class JobDetailsConfig : IEntityTypeConfiguration<JOB_DETAILS>
{
    public void Configure(EntityTypeBuilder<JOB_DETAILS> builder)
    {
        // 配置表名
        builder.ToTable($"qrtz_{nameof(JOB_DETAILS).ToLower()}");

        // 配置复合主键
        builder.HasKey(j => new { j.SchedName, j.JobName, j.JobGroup });

        // 配置各属性
        builder.Property(j => j.SchedName)
            .HasColumnName("sched_name")
            .IsRequired()
            .HasMaxLength(120); // 根据实际数据库调整长度

        builder.Property(j => j.JobName)
            .HasColumnName("job_name")
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(j => j.JobGroup)
            .HasColumnName("job_group")
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(j => j.Description)
            .HasColumnName("description")
            .HasMaxLength(250);

        builder.Property(j => j.JobClassName)
            .HasColumnName("job_class_name")
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(j => j.IsDurable)
            .HasColumnName("is_durable")
            .IsRequired();

        builder.Property(j => j.IsNonConcurrent)
            .HasColumnName("is_nonconcurrent")
            .IsRequired();

        builder.Property(j => j.IsUpdateData)
            .HasColumnName("is_update_data")
            .IsRequired();

        builder.Property(j => j.RequestsRecovery)
            .HasColumnName("requests_recovery")
            .IsRequired();

        builder.Property(j => j.JobData)
            .HasColumnName("job_data")
            .HasColumnType("longblob");

        // 配置与TRIGGERS的一对多关系
        builder.HasMany(j => j.Triggers)
            .WithOne(t => t.JobDetail)
            .HasForeignKey(t => new { t.SchedName, t.JobName, t.JobGroup })
            .HasPrincipalKey(j => new { j.SchedName, j.JobName, j.JobGroup });
    }
}
