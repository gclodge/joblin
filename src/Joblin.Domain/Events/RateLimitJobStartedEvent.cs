namespace Joblin.Domain.Events;

public class RateLimitJobStartedEvent(
    string rateLimitKey,
    string jobType,
    int activeJobCount,
    int jobsInCurrentWindow,
    int maxConcurrentJobs,
    int maxJobsPerTimeWindow)
    : BaseEvent
{
    public string RateLimitKey { get; } = rateLimitKey;
    public string JobType { get; } = jobType;
    public int ActiveJobCount { get; } = activeJobCount;
    public int JobsInCurrentWindow { get; } = jobsInCurrentWindow;
    public int MaxConcurrentJobs { get; } = maxConcurrentJobs;
    public int MaxJobsPerTimeWindow { get; } = maxJobsPerTimeWindow;
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
