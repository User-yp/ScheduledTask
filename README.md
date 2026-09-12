# ⏰ ScheduledTask

基于 **Quartz.NET 3.13 + .NET 8** 的分布式定时任务调度系统，支持集群部署、可视化管理、失败告警和自动重试。

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Quartz](https://img.shields.io/badge/Quartz-3.13.1-FF6B00)](https://www.quartz-scheduler.net/)
[![MySQL](https://img.shields.io/badge/MySQL-8.0-4479A1?logo=mysql)](https://www.mysql.com/)
[![Redis](https://img.shields.io/badge/Redis-7.0-DC382D?logo=redis)](https://redis.io/)

## ✨ 特性

- 🕐 **Cron 调度** — 基于标准 Cron 表达式（5段），Quartz 原生支持
- 🔗 **集群部署** — MySQL 持久化 + Quartz 集群，多节点高可用
- 📊 **Dashboard** — 内置 HTML 管理界面，可视化作业状态、执行历史和统计数据
- 🔔 **失败告警** — 作业失败时自动推送钉钉/飞书/企业微信 Webhook 通知
- 🔄 **配置热更新** — 修改 Cron 或参数后自动重新调度，无需重启服务
- 🔁 **自动重试** — 支持配置失败重试次数和延迟间隔
- 🔐 **JWT 认证** — API 端点受 JWT Bearer Token 保护
- 💚 **健康检查** — `/health` 端点，报告 Quartz / MySQL / Redis 状态
- 📝 **执行历史** — 持久化到 MySQL，支持按作业查询和自动清理
- 🚀 **双模式运行** — 支持 Console 和 WebApi 两种启动方式

## 🏗️ 项目结构

```
ScheduledTask.sln
├── ScheduledTask.Domain/         领域层 — 实体 + 接口
│   ├── IRepository/              IJob, ITriggerStore, ISchedulerManager, INotificationService
│   └── Persistence/              JobConfig, TriggerRecord, JobStats, Quartz 表实体
│
├── ScheduledTask.Infrastructure/ 基础设施层 — EF Core + Quartz 配置 + 业务逻辑
│   ├── DbContexts/               QuartzContext (MySQL)
│   ├── Config/                   EF Core 实体映射配置
│   ├── Repository/               DbTriggerStore, SchedulerManager, RegistService
│   ├── Listener/                 TriggerListener, JobListener, SchedulerListener
│   └── Notification/             WebhookNotificationService
│
├── ScheduledTask.Dll/            Redis + 动态配置加载
│   ├── Redis/                    IRedisService, RedisService, 扩展方法
│   └── Options/                  RedisOption, QuartzOption, NotificationOption
│
├── ScheduledTask.WebApi/         ASP.NET Core Web API
│   ├── Controllers/              AuthController, ManagerController, RegistController
│   ├── HealthChecks/             QuartzHealthCheck, RedisHealthCheck
│   └── wwwroot/                  dashboard.html
│
├── ScheduledTask.Console/        控制台宿主程序
│   ├── Program.cs                主机启动入口
│   └── ExampleJob.cs             示例作业
│
└── ScheduledTask/                旧版适配层（引用 Domain + Infrastructure）
    └── Migrations/               EF Core 数据库迁移
```

## 🛠️ 技术栈

| 组件 | 版本 | 用途 |
|------|------|------|
| .NET | 8.0 | 运行时 |
| Quartz.NET | 3.13.1 | 任务调度引擎 |
| EF Core + Pomelo | 8.0.22 / 8.0.3 | MySQL ORM |
| StackExchange.Redis | 2.8.58 | 配置缓存 |
| Swashbuckle | 6.6.2 | Swagger API 文档 |
| JWT Bearer | 8.0.0 | API 认证 |

## 🚀 快速开始

### 前置条件

- .NET 8 SDK
- MySQL 8.0+
- Redis 7.0+

### 1. 创建数据库

```sql
CREATE DATABASE Quartz CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```

### 2. 配置连接字符串

编辑 `ScheduledTask.Console/appsettings.json` 或 `ScheduledTask.WebApi/appsettings.json`：

```json
{
  "QuartzOption": {
    "SchedulerId": "1.0.1",
    "SchedulerName": "ScheduledTasks",
    "MaxConcurrency": 20,
    "TablePrefix": "qrtz_",
    "MysqlConnStr": "server=localhost;port=3306;database=Quartz;user=root;password=your_password;",
    "CheckinInterval": 60,
    "ConnectionString": "127.0.0.1:6379",
    "DbNumber": 1
  },
  "RedisOption": {
    "ConnectionString": "127.0.0.1:6379",
    "DbNumber": 0,
    "ConfigKey": "ScheduledTasksConfig"
  },
  "Jwt": {
    "ApiKey": "admin",
    "Secret": "ScheduledTaskSecretKey2024!@#$%"
  }
}
```

### 3. 运行数据库迁移

```bash
cd ScheduledTask
dotnet ef database update
```

### 4. 创建作业配置

在 `JOB_CONFIG` 表中插入配置（或通过 API 管理）：

```sql
INSERT INTO JOB_CONFIG (`GROUP`, JOB_KEYNAME, JOB_DESC, TRIGGER_KEYNAME, CRON, IS_ENABLE)
VALUES ('Default', 'ExampleJob', '示例作业', 'ExampleJob_Trigger', '0/30 * * * * ?', 'Y');
```

同步配置到 Redis：

```bash
curl -X POST http://localhost:5000/RegistController/SeedDataAsync
```

### 5. 启动服务

**Console 模式：**
```bash
cd ScheduledTask.Console
dotnet run
```

**WebApi 模式：**
```bash
cd ScheduledTask.WebApi
dotnet run
# 访问 http://localhost:5000/swagger
# 访问 http://localhost:5000/dashboard.html
```

## 📖 创建自定义作业

```csharp
using Quartz;
using ScheduledTask.Domain.IRepository;

public class MyJob : IDefaultJob
{
    private readonly ILogger<MyJob> logger;

    public MyJob(ILogger<MyJob> logger)
    {
        this.logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("MyJob 开始执行");

        // 从 JobDataMap 获取动态参数
        var apiUrl = context.MergedJobDataMap.GetString("ApiUrl");

        // 你的业务逻辑
        await Task.Delay(1000);

        logger.LogInformation("MyJob 执行完成");
    }
}
```

将 `MyJob` 放在 `ScheduledTask.Console` 或 `ScheduledTask.WebApi` 项目中，系统会自动发现并注册。

## 📡 API 文档

### 认证

| Method | 端点 | 说明 |
|--------|------|------|
| `POST` | `/Auth/Login` | 获取 JWT Token，Body: `{"apiKey":"admin"}` |

> 除 `/Auth/Login`、`/health`、`/dashboard.html` 外，所有管理端点需要 `Authorization: Bearer {token}` 请求头。

### 作业管理

| Method | 端点 | 说明 |
|--------|------|------|
| `GET` | `/ManagerController/GetAllJobAsync` | 获取所有作业列表 |
| `GET` | `/ManagerController/GetJobDetailAsync/{name}` | 获取作业详情 |
| `GET` | `/ManagerController/GetTriggersOfJobAsync/{name}` | 获取作业的触发器 |
| `GET` | `/ManagerController/GetTriggerStateAsync/{name}` | 获取触发器状态 |
| `POST` | `/ManagerController/TriggerJobAsync/{name}` | 立即触发作业 |
| `GET` | `/ManagerController/GetTriggerHistory/{name}` | 获取执行历史 |
| `POST` | `/ManagerController/AddTrigger/{name}/{cron}` | 添加新触发器 |
| `PUT` | `/ManagerController/ReplaceTrigger/{name}/{cron}` | 替换触发器 Cron |
| `GET` | `/ManagerController/GetNextTriggerTimeAsync/{name}` | 获取下次触发时间 |

### 统计

| Method | 端点 | 说明 |
|--------|------|------|
| `GET` | `/ManagerController/GetJobStats/{name}` | 单个作业统计 |
| `GET` | `/ManagerController/GetAllJobStats` | 全部作业统计 |

### 配置管理

| Method | 端点 | 说明 |
|--------|------|------|
| `POST` | `/RegistController/SeedDataAsync` | 从数据库加载配置到 Redis |
| `PUT` | `/RegistController/ReplaceTriggerAsync?jobkey=` | 替换触发器（热更新） |
| `PUT` | `/RegistController/UpdateJobData/{key}` | 更新作业参数（热更新） |
| `POST` | `/RegistController/RescheduleJob/{key}` | 手动热刷新作业 |

### 健康检查

| Method | 端点 | 说明 |
|--------|------|------|
| `GET` | `/health` | Quartz + Redis 健康状态 |

## ⚙️ JobConfig 配置说明

| 字段 | 类型 | 必填 | 说明 |
|------|------|:--:|------|
| `Group` | string | ✅ | 作业分组 |
| `JobKeyName` | string | ✅ | 作业名称（需与类名一致） |
| `JobDescription` | string | | 作业描述 |
| `TriggerKeyName` | string | ✅ | 触发器名称 |
| `TriggerDescription` | string | | 触发器描述 |
| `Cron` | string | ✅ | Cron 表达式 |
| `CronDescription` | string | | Cron 说明 |
| `IsEnable` | string | ✅ | 是否启用：`Y`/`N` |
| `JobData` | string | | 动态参数 JSON：`{"key":"value"}` |
| `MaxRetries` | int | | 失败重试次数，默认 0 |
| `RetryDelaySeconds` | int | | 重试间隔秒数，默认 60 |
| `TimeoutSeconds` | int | | 执行超时秒数，默认 0（不限制） |

## 🔔 通知告警配置

在 Redis `ScheduledTasksConfig` Hash 中添加 `NotificationOption`:

```json
{
  "Enabled": true,
  "WebhookUrl": "https://oapi.dingtalk.com/robot/send?access_token=xxx",
  "Platform": "dingtalk"
}
```

支持的平台：`dingtalk` / `feishu` / `wecom`

## 📊 Dashboard

访问 `http://localhost:5000/dashboard.html` 打开管理界面：

- 📈 顶部统计卡片：作业总数 / 总执行次数 / 整体成功率 / 失败次数
- 📋 作业列表：名称、分组、Cron、状态、成功率、平均耗时、操作按钮
- 🔍 点击"历史"查看每个作业的详细执行记录
- ⚡ 点击"触发"手动立即执行作业

## 🏥 健康检查

```bash
curl http://localhost:5000/health
# Healthy: Quartz 调度器正在运行, Redis 连接正常
```

## 📝 许可

MIT License

## 🤝 贡献

欢迎提交 Issue 和 Pull Request。
