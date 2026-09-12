using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduledTask.Infrastructure.Repository;
namespace ScheduledTask.WebApi.Controllers;

[ApiController]
[Route("[controller]/[action]")]
[Authorize]
public class RegistController : ControllerBase
{
    private readonly RegistService regist;
    private readonly ILogger<RegistController> logger;

    public RegistController(RegistService regist, ILogger<RegistController> logger)
    {
        this.regist = regist;
        this.logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> SeedDataAsync()
    {
        try
        {
            await regist.LoadConfigAsync();
            return Ok("Configuration loaded successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to load configuration");
            return StatusCode(500, "An error occurred while loading configuration.");
        }
    }

    [HttpPut]
    public async Task<IActionResult> ReplaceTriggerAsync(string jobkey)
    {
        try
        {
            if (string.IsNullOrEmpty(jobkey))
                return BadRequest("Job key cannot be null or empty.");
            if (await regist.ReplaceTriggerAsync(jobkey))
                return Ok("Trigger replaced successfully.");
            else
                return NotFound($"Job with key '{jobkey}' not found.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to replace trigger for job key '{JobKey}'", jobkey);
            return StatusCode(500, "An error occurred while replacing trigger.");
        }
    }

    [HttpPut("{jobkey}")]
    public async Task<IActionResult> UpdateJobData(string jobkey, [FromBody] Dictionary<string, string> jobData)
    {
        try
        {
            if (string.IsNullOrEmpty(jobkey))
                return BadRequest("Job key cannot be null or empty.");
            if (jobData == null || jobData.Count == 0)
                return BadRequest("JobData cannot be null or empty.");
            if (await regist.UpdateJobDataAsync(jobkey, jobData))
                return Ok("JobData updated successfully.");
            else
                return NotFound($"Job with key '{jobkey}' not found.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update JobData for job key '{JobKey}'", jobkey);
            return StatusCode(500, "An error occurred while updating JobData.");
        }
    }

    [HttpPost("{jobkey}")]
    public async Task<IActionResult> RescheduleJob(string jobkey)
    {
        try
        {
            if (string.IsNullOrEmpty(jobkey))
                return BadRequest("Job key cannot be null or empty.");
            if (await regist.RescheduleJobAsync(jobkey))
                return Ok("Job rescheduled successfully.");
            else
                return NotFound($"Job with key '{jobkey}' not found.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to reschedule job '{JobKey}'", jobkey);
            return StatusCode(500, "An error occurred while rescheduling job.");
        }
    }
}
