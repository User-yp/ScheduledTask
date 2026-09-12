using Quartz;
using ScheduledTask.Domain.Persistence;

namespace ScheduledTask.Domain.IRepository;

public interface ISchedulerManager
{
    Task<List<string>?> GetAllGroupAsync(CancellationToken cancellationToken = default);
    Task<List<JobKey>?> GetAllJobsAsync(CancellationToken cancellationToken = default);
    Task<List<JobKey>?> GetJobsByGroupAsync(string groupName, CancellationToken cancellationToken = default);
    Task<JobKey?> GetJobAsync(string jobName, CancellationToken cancellationToken = default);
    Task<bool> PauseAllJobAsync(CancellationToken cancellationToken = default);
    Task<bool> PauseJobsAsync(List<JobKey> jobKeys, CancellationToken cancellationToken = default);
    Task<bool> PauseJobsByGroupAsync(string groupName, CancellationToken cancellationToken = default);
    Task<bool> ResumeAllJobAsync(CancellationToken cancellationToken = default);
    Task<bool> ResumeJobAsync(JobKey jobKey, CancellationToken cancellationToken = default);
    Task<bool> ResumeJobsAsync(List<JobKey> jobKeys, CancellationToken cancellationToken = default);
    Task<bool> ResumeJobsByGroupAsync(string groupName, CancellationToken cancellationToken = default);
    Task<bool> TriggerJobAsync(JobKey jobKey, CancellationToken cancellationToken = default);
    Task<bool> TriggerJobsAsync(List<JobKey> jobKeys, CancellationToken cancellationToken = default);
    Task<bool> DeleteAllJobAsync(JobKey jobKey, CancellationToken cancellationToken = default);
    Task<bool> DeleteJobAsync(JobKey jobKey, CancellationToken cancellationToken = default);
    Task<bool> DeleteJobsAsync(List<JobKey> jobKeys, CancellationToken cancellationToken = default);
    Task<bool> DeleteJobByGroupAsync(string groupName, CancellationToken cancellationToken = default);
    Task<ITrigger?> GetTriggerAsync(TriggerKey triggerKey, CancellationToken cancellationToken = default);
    Task<List<ITrigger>?> GetTriggersOfJobAsync(JobKey jobKey, CancellationToken cancellationToken = default);
    Task<TriggerState?> GetTriggerStateAsync(TriggerKey triggerKey, CancellationToken cancellationToken = default);
    Task<IJobDetail?> GetJobDetailAsync(JobKey jobKey, CancellationToken cancellationToken = default);
    Task<IJobDetail?> CreateJobDetailAsync(JobKey jobKey, CancellationToken cancellationToken = default);
    Task<bool> CreateNewJobAsync(JobConfig jobConfig, Type? type, CancellationToken cancellationToken = default);
    Task<List<TriggerRecord>> GetTriggerHistoryAsync(JobKey jobKey, int maxRecords = 100, CancellationToken cancellationToken = default);
    Task<bool> AddTriggerToJobAsync(JobKey jobKey, string cron, CancellationToken cancellationToken = default);
    Task<bool> ReplaceTriggerAsync(JobKey jobKey, string cron, CancellationToken cancellationToken = default);
    Task<DateTime> GetNextTriggerTimeAsync(JobKey jobKey, int maxRecords = 100, CancellationToken cancellationToken = default);
    Task<JobStats?> GetJobStatsAsync(JobKey jobKey, CancellationToken cancellationToken = default);
    Task<List<JobStats>> GetAllJobStatsAsync(CancellationToken cancellationToken = default);
}
