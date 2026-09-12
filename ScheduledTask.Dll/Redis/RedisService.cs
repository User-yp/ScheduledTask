using ScheduledTask.Dll.Options;
using StackExchange.Redis;
using System.Collections.Concurrent;

namespace ScheduledTask.Dll.Redis;

public class RedisService : IRedisService
{
    private readonly ConnectionMultiplexer _conn;
    private readonly IDatabase _db;
    public RedisService(RedisOption option)
    {
        //var option= options.CurrentValue;
        var connectionString = option.ConnectionString;
        _conn = ConnectionMultiplexer.Connect(connectionString);

        var dbNumber = option.DbNumber;
        _db = _conn.GetDatabase(dbNumber);
    }
    public async Task<bool> PingAsync()
    {
        try
        {
            await _db.PingAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    #region Hash

    public async Task<ConcurrentDictionary<string, string>> HashGetAsync(string key)
    {
        return (await _db.HashGetAllAsync(key)).ToConcurrentDictionary();
    }
    public async Task<ConcurrentDictionary<string, string>> HashGetFieldsAsync(string key, IEnumerable<string> fields)
    {
        return (await _db.HashGetAsync(key, fields.ToRedisValues())).ToConcurrentDictionary(fields);
    }
    public async Task<ConcurrentDictionary<string, string>> HashGetFieldAsync(string key, string fields)
    {
        return (await _db.HashGetAsync(key, fields.ToRedisValue())).ToConcurrentDictionary(fields);
    }
    public async Task HashSetAsync(string key, ConcurrentDictionary<string, string> entries)
    {
        var val = entries.ToHashEntries();
        if (val != null)
            await _db.HashSetAsync(key, val);
    }

    public async Task HashSetFieldsAsync(string key, ConcurrentDictionary<string, string> fields)
    {
        if (fields == null || fields.IsEmpty)
            return;

        var hs = await HashGetAsync(key);
        foreach (var field in fields)
        {
            //if(!hs.ContainsKey(field.Key))

            //    continue;

            hs[field.Key] = field.Value;
        }
        await HashSetAsync(key, hs);
    }
    public async Task<bool> HashSetFieldAsync(string key, ConcurrentDictionary<string, string> fields)
    {
        try
        {
            if (fields != null && !fields.IsEmpty)
                await HashSetAsync(key, fields);
            //pengye 2025-05-23 优化日志记录
            //log.LogInfo(nameof(HashSetFieldAsync), LogMessage.HashSetSucess, key);
            return true;
            /*if (!await KeyExistsAsync(key))
                await HashSetAsync(key, fields);
            else
            {
                if (fields == null || fields.IsEmpty)
                    return true;

                var hs = await HashGetAsync(key);
                foreach (var field in fields)
                {
                    //if(!hs.ContainsKey(field.Key))
                    //    continue;
                    hs[field.Key] = field.Value;
                }
                await HashSetAsync(key, hs);
            }
            log.LogInfo(nameof(HashSetFieldAsync), LogMessage.HashSetSucess, key);
            return true;*/
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<bool> HashFieldsExistsAsync(string key, IEnumerable<string> fields)
    {
        if (!await KeyExistsAsync(key))
            return false;
        var dic = await HashGetFieldsAsync(key, fields);
        foreach (var field in fields)
        {
            if (dic[field] == null)
                return false;
        }
        return true;
    }

    public async Task<long> HashDeleteFieldsAsync(string key, IEnumerable<string> fields)
    {
        try
        {
            if (fields == null || !fields.Any())
                return -1;

            var count = await _db.HashDeleteAsync(key, fields.ToRedisValues());
            return count;
        }
        catch (Exception ex)
        {
            throw;
        }
        /*if (fields == null || !fields.Any())
            return false;

        var success = true;
        foreach (var field in fields)
        {
            if (!await _db.HashDeleteAsync(key, field))
                success = false;
        }
        return success;*/
    }
    #endregion


    #region List
    /// <summary>
    /// 将值插入列表尾部
    /// </summary>
    public async Task<long> ListRightPushAsync(string key, string value)
    {
        return await _db.ListRightPushAsync(key, value);
    }

    /// <summary>
    /// 移除并返回列表的第一个元素
    /// </summary>
    public async Task<string> ListLeftPopAsync(string key)
    {
        var result = await _db.ListLeftPopAsync(key);
        return result.HasValue ? result.ToString() : null;
    }
    #endregion

    #region Key

    public async Task<long> GetHashLength(string key)
    {
        if (!await KeyExistsAsync(key))
            return 0;
        return await _db.HashLengthAsync(key);
    }

    public async Task<bool> KeyExistsAsync(string key)
    {
        return await _db.KeyExistsAsync(key);
    }

    public async Task<long> KeyDeleteAsync(IEnumerable<string> keys)
    {
        return await _db.KeyDeleteAsync(keys.Select(k => (RedisKey)k).ToArray());
    }

    public async Task<bool> KeyDeleteAsync(string key)
    {
        return await _db.KeyDeleteAsync(key);
    }
    #endregion

}
