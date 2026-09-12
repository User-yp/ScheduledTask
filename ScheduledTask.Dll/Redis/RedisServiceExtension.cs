using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Collections.Concurrent;

namespace ScheduledTask.Dll.Redis;

public static class RedisServiceExtension
{
    public static IServiceCollection AddRedis(this IServiceCollection services)//, IConfiguration configuration
    {
        //services.Configure<RedisOption>(configuration.GetSection(nameof(RedisOption)));
        services.AddSingleton<IRedisService, RedisService>();
        return services;
    }

    public static void AddKey(this List<string> strings, string key)
    {
        if (!strings.Contains(key))
        {
            strings.Add(key);
        }
    }

    public static RedisValue ToRedisValue<T>(this T value)
    {
        if (value == null)
            return RedisValue.Null;

        return value switch
        {
            ValueType => value.ToString(),
            string s => s,
            _ => JsonConvert.SerializeObject(value)
        };
    }

    public static RedisValue[] ToRedisValues<T>(this IEnumerable<T> values)
    {
        var enumerable = values as T[] ?? values.ToArray();
        return !enumerable.Any() ? null : enumerable.Select(v => v.ToRedisValue()).ToArray();
    }

    public static HashEntry[] ToHashEntries(this ConcurrentDictionary<string, string> entries)
    {
        if (entries == null || !entries.Any())
            return null;

        var es = new HashEntry[entries.Count];
        for (var i = 0; i < entries.Count; i++)
        {
            var name = entries.Keys.ElementAt(i);
            var value = entries[name];
            es[i] = new HashEntry(name, value);
        }

        return es;
    }

    public static ConcurrentDictionary<string, string> ToConcurrentDictionary(this IEnumerable<HashEntry> entries)
    {
        var hashEntries = entries as HashEntry[] ?? entries.ToArray();
        if (!hashEntries.Any())
            return null;


        var dict = new ConcurrentDictionary<string, string>();
        foreach (var entry in hashEntries)
            dict[entry.Name] = entry.Value;

        return dict;
    }
    public static ConcurrentDictionary<string, string> ToConcurrentDictionary(this RedisValue hashValues,
       string fields)
    {
        var dict = new ConcurrentDictionary<string, string>();
        dict[fields] = hashValues;
        return dict;
    }
    public static ConcurrentDictionary<string, string> ToConcurrentDictionary(this RedisValue[] hashValues,
        IEnumerable<string> fields)
    {
        var enumerable = fields as string[] ?? fields.ToArray();
        if (hashValues == null || !hashValues.Any() || !enumerable.Any())
            return null;

        var dict = new ConcurrentDictionary<string, string>();
        for (var i = 0; i < enumerable.Count(); i++)
            dict[enumerable.ElementAt(i)] = hashValues[i];

        return dict;
    }
}
