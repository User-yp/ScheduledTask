using System.Collections.Concurrent;

namespace ScheduledTask.Dll.Redis;

public interface IRedisService
{
    Task<ConcurrentDictionary<string, string>> HashGetAsync(string key);
    Task<ConcurrentDictionary<string, string>> HashGetFieldsAsync(string key, IEnumerable<string> fields);
    Task<ConcurrentDictionary<string, string>> HashGetFieldAsync(string key, string fields);
    Task HashSetAsync(string key, ConcurrentDictionary<string, string> entries);
    Task HashSetFieldsAsync(string key, ConcurrentDictionary<string, string> fields);
    Task<bool> HashSetFieldAsync(string key, ConcurrentDictionary<string, string> fields);
    Task<bool> HashFieldsExistsAsync(string key, IEnumerable<string> fields);
    Task<long> HashDeleteFieldsAsync(string key, IEnumerable<string> fields);
    Task<long> GetHashLength(string key);
    Task<bool> KeyExistsAsync(string key);
    Task<long> KeyDeleteAsync(IEnumerable<string> keys);
    Task<bool> KeyDeleteAsync(string key);

    // 健康检查
    Task<bool> PingAsync();

    // List 操作
    Task<long> ListRightPushAsync(string key, string value);
    Task<string> ListLeftPopAsync(string key);
}
