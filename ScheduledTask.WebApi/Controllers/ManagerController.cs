using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quartz;
using ScheduledTask.Domain.IRepository;
using ScheduledTask.Infrastructure;
namespace ScheduledTask.WebApi.Controllers;

[ApiController]
[Route("[controller]/[action]")]
[Authorize]
public class ManagerController : ControllerBase
{

    private readonly ISchedulerManager manager;
    private readonly IScheduler scheduler;

    public ManagerController(IServiceProvider serviceProvider)
    {
        manager = serviceProvider.GetRequiredService<ISchedulerManager>();
        scheduler = serviceProvider.GetRequiredService<IScheduler>();
    }

    [HttpGet]
    public IActionResult GetTimeNow()
    {
        return Ok(DateTime.Now.ToString());
    }

    [HttpGet]
    public async Task<ActionResult<List<JobKey?>>> GetAllJobAsync()
    {
        var jobs = await manager.GetAllJobsAsync();
        if (jobs == null || jobs.Count == 0)
            return BadRequest();
        return Ok(jobs);
    }

    [HttpGet("{jobname}")]
    public async Task<IActionResult> GetJobDetailAsync(string jobname)
    {
        try
        {
            var job = await manager.GetJobAsync(jobname);
            if (job == null)
                return BadRequest();
            var jobDetail = await manager.GetJobDetailAsync(job);
            if (jobDetail == null)
                return BadRequest("没有JobDetail");
            return Ok(jobDetail);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{jobname}")]
    public async Task<ActionResult<List<ITrigger>>> GetTriggersOfJobAsync(string jobname)
    {
        var job = await manager.GetJobAsync(jobname);
        if (job == null)
            return BadRequest();
        var triggers = await manager.GetTriggersOfJobAsync(job);
        if (triggers == null)
            return BadRequest("没有触发器");
        return Ok(triggers);
    }

    [HttpGet("{jobname}")]
    public async Task<ActionResult<string>> GetTriggerStateAsync(string jobname)
    {
        var job = await manager.GetJobAsync(jobname);
        if (job == null)
            return BadRequest();
        var state = await manager.GetTriggerStateAsync(new TriggerKey(job.Name, job.Group));
        if (state == null)
            return BadRequest("获取触发器状态失败");
        return Ok(state);
    }

    [HttpPost("{jobname}")]
    public async Task<ActionResult<string>> TriggerJobAsync(string jobname)
    {
        var job = await manager.GetJobAsync(jobname);
        if (job == null)
            return BadRequest();
        var isTrigger = await manager.TriggerJobAsync(job);
        if (!isTrigger)
            return BadRequest("任务触发失败");
        return Ok("任务触发成功");
    }

    [HttpGet("{jobname}")]
    public async Task<IActionResult> GetTriggerHistory(string jobname)
    {
        var job = await manager.GetJobAsync(jobname);
        if (job == null)
            return BadRequest();
        var history = await manager.GetTriggerHistoryAsync(job);
        return Ok(history);
    }

    [HttpPost("{jobname}/{cron}")]
    public async Task<IActionResult> AddTrigger(string jobname, string cron)
    {
        if (!cron.ValidateCron())
            return BadRequest("无效的 Cron 表达式");
        var job = await manager.GetJobAsync(jobname);
        if (job == null)
            return BadRequest("作业不存在");
        if (await manager.AddTriggerToJobAsync(job, cron))
            return Ok("触发器添加成功");
        return BadRequest("触发器添加失败");
    }

    [HttpPut("{jobname}/{cron}")]
    public async Task<IActionResult> ReplaceTrigger(string jobname, string cron)
    {
        if (!cron.ValidateCron())
            return BadRequest("无效的 Cron 表达式");
        var job = await manager.GetJobAsync(jobname);
        if (job == null)
            return BadRequest("作业不存在");
        if (await manager.ReplaceTriggerAsync(job, cron))
            return Ok("触发器替换成功");
        return BadRequest("触发器替换失败");
    }

    [HttpGet("{jobname}")]
    public async Task<IActionResult> GetNextTriggerTimeAsync(string jobname)
    {
        var job = await manager.GetJobAsync(jobname);
        if (job == null)
            return BadRequest("作业不存在");
        var nextTime = await manager.GetNextTriggerTimeAsync(job);
        return Ok(nextTime);
    }

    [HttpGet("{jobname}")]
    public async Task<IActionResult> GetJobStats(string jobname)
    {
        var job = await manager.GetJobAsync(jobname);
        if (job == null)
            return BadRequest("作业不存在");
        var stats = await manager.GetJobStatsAsync(job);
        if (stats == null)
            return BadRequest("暂无统计数据");
        return Ok(stats);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllJobStats()
    {
        var stats = await manager.GetAllJobStatsAsync();
        return Ok(stats);
    }
}
