using Microsoft.AspNetCore.Mvc;

namespace Joblin.Controllers;

[ApiController]
[Route("api/joblin/webhooks")]
public class JoblinWebhookController(
    IJoblinManager joblinManager) : ControllerBase
{
    private readonly IJoblinManager _joblinManager = joblinManager ?? throw new ArgumentNullException(nameof(joblinManager));

    [HttpPost("status")]
    public async Task<IActionResult> UpdateJobStatus([FromBody] JobStatusUpdate update)
    {
        if (string.IsNullOrEmpty(update.JobId)) return BadRequest("JobId is required");

        try
        {
            await _joblinManager.UpdateJobStatusAsync(
                update.JobId, 
                update.Status, 
                update.Result, 
                update.ErrorMessage);

            return Ok(new { success = true, jobId = update.JobId });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("heartbeat/{jobId}")]
    public async Task<IActionResult> Heartbeat(
        string jobId,
        [FromBody] JobHeartbeat? heartbeat = null)
    {
        try
        {
            await _joblinManager.RecordHeartbeatAsync(jobId, heartbeat?.Progress, heartbeat?.Message);
            return Ok(new { success = true, timestamp = DateTime.UtcNow });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("jobs/{jobId}/status")]
    public async Task<IActionResult> GetJobStatus(string jobId)
    {
        try
        {
            var status = await _joblinManager.GetJobStatusAsync(jobId);
            return Ok(new { jobId, status });
        }
        catch (Exception ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpGet("jobs")]
    public async Task<IActionResult> GetJobs(
        [FromQuery] JobStatus? status = null,
        [FromQuery] int? limit = null)
    {
        try
        {
            var filter = new JobFilter
            {
                Status = status,
                Limit = limit
            };

            var jobs = await _joblinManager.GetJobsAsync(filter);
            return Ok(jobs);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
