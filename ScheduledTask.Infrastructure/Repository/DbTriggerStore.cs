using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using ScheduledTask.Domain.IRepository;
using ScheduledTask.Domain.Persistence;
using ScheduledTask.Infrastructure.DbContexts;

namespace ScheduledTask.Infrastructure.Repository;

public class DbTriggerStore : ITriggerStore
{
    private readonly IDbContextFactory<QuartzContext> contextFactory;
    private readonly ILogger<DbTriggerStore> logger;
    private const int DefaultMaxRecordsPerJob = 1000;

    public DbTriggerStore(IDbContextFactory<QuartzContext> contextFactory, ILogger<DbTriggerStore> logger)
    {
        this.contextFactory = contextFactory;
        this.logger = logger;
    }

    public async Task AddRecordAsync(TriggerRecord record, CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        context.TriggerRecords.Add(record);
        await context.SaveChangesAsync(cancellationToken);

        // 自动清理：每个 JobKey 只保留最近 N 条记录
        await CleanupOldRecordsAsync(record.JobKey, DefaultMaxRecordsPerJob, cancellationToken);
    }

    public async Task<List<TriggerRecord>> GetRecordsAsync(JobKey jobKey, int maxRecords, CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.TriggerRecords
            .AsNoTracking()
            .Where(r => r.JobKey == jobKey.Name)
            .OrderByDescending(r => r.FireTime)
            .Take(maxRecords)
            .ToListAsync(cancellationToken);
    }

    private async Task CleanupOldRecordsAsync(string jobKey, int maxRecords, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            var totalCount = await context.TriggerRecords
                .CountAsync(r => r.JobKey == jobKey, cancellationToken);

            if (totalCount <= maxRecords)
                return;

            var toDelete = totalCount - maxRecords;
            var oldRecords = await context.TriggerRecords
                .Where(r => r.JobKey == jobKey)
                .OrderBy(r => r.FireTime)
                .Take(toDelete)
                .ToListAsync(cancellationToken);

            if (oldRecords.Count > 0)
            {
                context.TriggerRecords.RemoveRange(oldRecords);
                await context.SaveChangesAsync(cancellationToken);
                logger.LogDebug("Cleaned up {Count} old trigger records for job {JobKey}", oldRecords.Count, jobKey);
            }
        }
        catch (Exception ex)
        {
            // 清理失败不影响主流程
            logger.LogWarning(ex, "Failed to cleanup old trigger records for job {JobKey}", jobKey);
        }
    }
}
