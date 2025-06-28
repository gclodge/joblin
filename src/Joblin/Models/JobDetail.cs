namespace Joblin.Models;

public class JobDetail
{
    public string Id { get; set; } = string.Empty;
    public object? Data { get; set; }
    public string WebhookUrl { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public JobStatus Status { get; set; }
    public object? Result { get; set; }
    public string? ErrorMessage { get; set; }
    public JobOptions Options { get; set; } = new();
}
