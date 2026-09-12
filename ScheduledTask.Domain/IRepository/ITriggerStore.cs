using Quartz;
using ScheduledTask.Domain.Persistence;

namespace ScheduledTask.Domain.IRepository;

public interface ITriggerStore
{
    Task AddRecordAsync(TriggerRecord record, CancellationToken cancellationToken = default);
    Task<List<TriggerRecord>> GetRecordsAsync(JobKey jobKey, int maxRecords, CancellationToken cancellationToken = default);
}
