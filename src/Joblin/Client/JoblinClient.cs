using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Joblin.Models;

namespace Joblin.Client;

public class JoblinClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public JoblinClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    }

    public async Task UpdateJobStatusAsync(string webhookUrl, string jobId, JobStatus status, object? result = null, string? errorMessage = null)
    {
        var update = new JobStatusUpdate
        {
            JobId = jobId,
            Status = status,
            Result = result,
            ErrorMessage = errorMessage,
            Timestamp = DateTime.UtcNow
        };

        var json = JsonSerializer.Serialize(update, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync(webhookUrl, content);
        response.EnsureSuccessStatusCode();
    }

    public async Task SendHeartbeatAsync(string webhookUrl, string jobId, int? progress = null, string? message = null)
    {
        var heartbeat = new { progress, message };
        var json = JsonSerializer.Serialize(heartbeat, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var heartbeatUrl = webhookUrl.Replace("/status", $"/heartbeat/{jobId}");
        var response = await _httpClient.PostAsync(heartbeatUrl, content);
        response.EnsureSuccessStatusCode();
    }

    public async Task<JobStatus> GetJobStatusAsync(string baseUrl, string jobId)
    {
        var statusUrl = $"{baseUrl.TrimEnd('/')}/api/joblin/webhooks/jobs/{jobId}/status";
        var response = await _httpClient.GetAsync(statusUrl);
        response.EnsureSuccessStatusCode();
        
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<dynamic>(json, _jsonOptions);
        
        return Enum.Parse<JobStatus>(result?.status?.ToString() ?? "Queued");
    }
}
