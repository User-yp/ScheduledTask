using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScheduledTask.Domain.Persistence;

namespace ScheduledTask.Infrastructure.Config;

internal class TriggerConfig : IEntityTypeConfiguration<TRIGGERS>
{
    public void Configure(EntityTypeBuilder<TRIGGERS> builder)
    {
        builder.ToTable($"qrtz_{nameof(TRIGGERS).ToLower()}");
        // 配置复合主键
        builder.HasKey(e => new { e.SchedName, e.TriggerName, e.TriggerGroup });

        // 配置属性
        builder.Property(e => e.SchedName)
            .HasColumnName("sched_name")
            .IsRequired();

        builder.Property(e => e.TriggerName)
            .HasColumnName("trigger_name")
            .IsRequired();

        builder.Property(e => e.TriggerGroup)
            .HasColumnName("trigger_group")
            .IsRequired();

        builder.Property(e => e.JobName)
            .HasColumnName("job_name");

        builder.Property(e => e.JobGroup)
            .HasColumnName("job_group");

        builder.Property(e => e.Description)
            .HasColumnName("description");

        builder.Property(e => e.NextFireTime)
            .HasColumnName("next_fire_time");

        builder.Property(e => e.PrevFireTime)
            .HasColumnName("prev_fire_time");

        builder.Property(e => e.Priority)
            .HasColumnName("priority");

        builder.Property(e => e.TriggerState)
            .HasColumnName("trigger_state");

        builder.Property(e => e.TriggerType)
            .HasColumnName("trigger_type");

        builder.Property(e => e.StartTime)
            .HasColumnName("start_time");

        builder.Property(e => e.EndTime)
            .HasColumnName("end_time");

        builder.Property(e => e.CalendarName)
            .HasColumnName("calendar_name");

        builder.Property(e => e.MisfireInstr)
            .HasColumnName("misfire_instr");

        builder.Property(e => e.JobData)
            .HasColumnName("job_data")
            .HasColumnType("longblob");


        // 配置外键关系
        builder.HasOne(t => t.JobDetail)
           .WithMany(j => j.Triggers)
           .HasForeignKey(t => new { t.SchedName, t.JobName, t.JobGroup })
           .HasConstraintName("FK_TRIGGERS_JOB_DETAILS")
           .HasPrincipalKey(j => new { j.SchedName, j.JobName, j.JobGroup });
    }
}
