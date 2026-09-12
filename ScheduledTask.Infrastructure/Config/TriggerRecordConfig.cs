using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScheduledTask.Domain.Persistence;

namespace ScheduledTask.Infrastructure.Config;

internal class TriggerRecordConfig : IEntityTypeConfiguration<TriggerRecord>
{
    public void Configure(EntityTypeBuilder<TriggerRecord> builder)
    {
        builder.ToTable("TRIGGER_RECORDS");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("ID")
            .ValueGeneratedOnAdd();

        builder.Property(r => r.SchedulerName)
            .HasColumnName("SCHEDULER_NAME")
            .HasMaxLength(120);

        builder.Property(r => r.SchedulerId)
            .HasColumnName("SCHEDULER_ID")
            .HasMaxLength(120);

        builder.Property(r => r.FireTime)
            .HasColumnName("FIRE_TIME")
            .IsRequired();

        builder.Property(r => r.TriggerKey)
            .HasColumnName("TRIGGER_KEY")
            .HasMaxLength(200);

        builder.Property(r => r.JobKey)
            .HasColumnName("JOB_KEY")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(r => r.Duration)
            .HasColumnName("DURATION");

        builder.Property(r => r.Success)
            .HasColumnName("SUCCESS");

        builder.Property(r => r.ErrorMessage)
            .HasColumnName("ERROR_MESSAGE")
            .HasColumnType("longtext");

        // Index for efficient query by JobKey ordered by FireTime
        builder.HasIndex(r => new { r.JobKey, r.FireTime })
            .HasDatabaseName("IX_TRIGGER_RECORDS_JOB_FIRE");
    }
}
