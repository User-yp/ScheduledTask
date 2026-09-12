using Microsoft.EntityFrameworkCore;
using ScheduledTask.Domain.Persistence;

namespace ScheduledTask;

public class QuartzContext : DbContext
{
    public DbSet<JobConfig> JobConfig { get; set; }
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

    public QuartzContext(DbContextOptions<QuartzContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 扫描本程序集和 Infrastructure 程序集加载实体配置
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        var infraAssembly = System.Reflection.Assembly.Load("ScheduledTask.Infrastructure");
        modelBuilder.ApplyConfigurationsFromAssembly(infraAssembly);
    }
}

