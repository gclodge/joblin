namespace Joblin.Models;

public class JoblinWebhookResponse
{
    public bool Success { get; set; }
    public string? JobId { get; set; }
    public object? Data { get; set; }
    public string? Error { get; set; }

    public static JoblinWebhookResponse CreateSuccess(string jobId, object? data = null) =>
        new() { Success = true, JobId = jobId, Data = data };

    public static JoblinWebhookResponse CreateError(string error) =>
        new() { Success = false, Error = error };
}
