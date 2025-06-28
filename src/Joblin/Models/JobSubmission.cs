namespace Joblin.Models;

public class JobSubmission
{
    public string Id { get; set; } = string.Empty;
    public object? Data { get; set; }
    public string WebhookUrl { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public JobStatus Status { get; set; }
    public JobOptions Options { get; set; } = new();
}
