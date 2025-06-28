namespace Joblin.Models;

public class JobSummary
{
    public string Id { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public JobStatus Status { get; set; }
    public int Priority { get; set; }
}
