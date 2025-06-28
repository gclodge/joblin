namespace Joblin.Models;

public class JobStatusUpdate
{
    public string JobId { get; set; } = string.Empty;
    public JobStatus Status { get; set; }
    public object? Result { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime Timestamp { get; set; }
}
