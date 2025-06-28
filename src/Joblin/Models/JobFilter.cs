namespace Joblin.Models;

public class JobFilter
{
    public JobStatus? Status { get; set; }
    public DateTime? SubmittedAfter { get; set; }
    public DateTime? SubmittedBefore { get; set; }
    public int? Limit { get; set; }
}
