using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using ScheduledTask.Dll.Options;
using ScheduledTask.Dll.Redis;
using StackExchange.Redis;
using System.Reflection;

namespace ScheduledTask.Dll;

public static class DllExtension
{
    public static IServiceCollection LoadConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var opt = configuration.GetSection(nameof(RedisOption)).Get<RedisOption>()
            ?? throw new ArgumentNullException(nameof(RedisOption), "RedisOption configuration is null or empty.");
        // 创建临时连接
        var tempConnection = ConnectionMultiplexer.Connect(opt.ConnectionString);

        try
        {
            var db = tempConnection.GetDatabase(opt.DbNumber);
            var configs = db.HashGetAll(opt.ConfigKey).ToConcurrentDictionary();
            if (configs == null || configs.Count == 0)
            {
                // Redis 中没有配置数据，可能尚未初始化
                return services;
            }

            var optionTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => !t.IsAbstract && t.IsClass && t.GetCustomAttributes(typeof(OptionAttribute), false).Length != 0).ToList();

            foreach (var item in configs)
            {
                var type = optionTypes.FirstOrDefault(t => t.Name.Equals(item.Key.ToString(), StringComparison.OrdinalIgnoreCase));
                if (type == null)
                    continue;

                var config = JsonConvert.DeserializeObject(item.Value, type)
                    ?? throw new ArgumentNullException(nameof(item.Value), $"Configuration for {item.Key} is null or empty.");
                //将配置注入到容器里
                services.AddSingleton(type, config);
            }
        }
        finally
        {
            tempConnection.Close();
        }
        return services;
    }
    public static IServiceCollection RegistDllService(this IServiceCollection services)//,IConfiguration configuration
    {
        services.AddRedis();//configuration
        return services;
    }
}
