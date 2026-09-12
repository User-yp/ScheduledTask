using Microsoft.EntityFrameworkCore;
using ScheduledTask.Domain.Persistence;

namespace ScheduledTask.Infrastructure.DbContexts;

public class QuartzContext : DbContext
{
    public DbSet<JobConfig> JobConfigs { get; set; }
    public DbSet<BLOB_TRIGGERS> BlobTriggers { get; set; }
    public DbSet<CALENDARS> Calendars { get; set; }
    public DbSet<CRON_TRIGGERS> CronTriggers { get; set; }
    public DbSet<FIRED_TRIGGERS> FiredTriggers { get; set; }
    public DbSet<JOB_DETAILS> JobDetails { get; set; }
    public DbSet<LOCKS> Locks { get; set; }
    public DbSet<PAUSED_TRIGGER_GRPS> PausedTriggerGrps { get; set; }
    public DbSet<SCHEDULER_STATE> SchedulerStates { get; set; }
    public DbSet<SIMPLE_TRIGGERS> SimpleTriggers { get; set; }
    public DbSet<SIMPROP_TRIGGERS> SimpropTriggers { get; set; }
    public DbSet<TRIGGERS> Triggers { get; set; }
    public DbSet<TriggerRecord> TriggerRecords { get; set; }

    public QuartzContext(DbContextOptions<QuartzContext> options) : base(options)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Adds Quartz.NET SqlServer schema to EntityFrameworkCore
        //modelBuilder.AddQuartz(builder => builder.UsePostgreSql("qrtz_","public"));
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}