using Microsoft.AspNetCore.Http;

using Joblin.Models;

namespace Joblin.Abstractions;

public interface IJoblinWebhookService
{
    Task<JoblinWebhookResponse> HandleStatusUpdateAsync(JobStatusUpdate update);
    Task<JoblinWebhookResponse> HandleHeartbeatAsync(string jobId, JobHeartbeat? heartbeat = null);
    Task<JoblinWebhookResponse> HandleJobQueryAsync(string jobId);
    string GenerateWebhookUrl(HttpRequest request, string jobId);
}
