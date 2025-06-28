using Microsoft.AspNetCore.Http;

namespace Joblin.Services;

public class JoblinWebhookService(
    IJoblinManager joblinManager,
    JoblinOptions options)
    : IJoblinWebhookService
{
    private readonly IJoblinManager _joblinManager = joblinManager ?? throw new ArgumentNullException(nameof(joblinManager));
    private readonly JoblinOptions _options = options ?? throw new ArgumentNullException(nameof(options));

    public async Task<JoblinWebhookResponse> HandleStatusUpdateAsync(JobStatusUpdate update)
    {
        try
        {
            await _joblinManager.UpdateJobStatusAsync(
                update.JobId, 
                update.Status, 
                update.Result, 
                update.ErrorMessage);

            return JoblinWebhookResponse.CreateSuccess(update.JobId);
        }
        catch (Exception ex)
        {
            return JoblinWebhookResponse.CreateError(ex.Message);
        }
    }

    public async Task<JoblinWebhookResponse> HandleHeartbeatAsync(string jobId, JobHeartbeat? heartbeat = null)
    {
        try
        {
            await _joblinManager.RecordHeartbeatAsync(jobId, heartbeat?.Progress, heartbeat?.Message);
            return JoblinWebhookResponse.CreateSuccess(jobId, new { timestamp = DateTime.UtcNow });
        }
        catch (Exception ex)
        {
            return JoblinWebhookResponse.CreateError(ex.Message);
        }
    }

    public async Task<JoblinWebhookResponse> HandleJobQueryAsync(string jobId)
    {
        try
        {
            var status = await _joblinManager.GetJobStatusAsync(jobId);
            return JoblinWebhookResponse.CreateSuccess(jobId, new { status });
        }
        catch (Exception ex)
        {
            return JoblinWebhookResponse.CreateError(ex.Message);
        }
    }

    public string GenerateWebhookUrl(HttpRequest request, string jobId)
    {
        var baseUrl = $"{request.Scheme}://{request.Host}";
        return $"{baseUrl}/{_options.WebhookEndpoints.BaseRoute}/{_options.WebhookEndpoints.StatusUpdateRoute}";
    }
}
