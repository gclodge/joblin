namespace Joblin.Domain.Events;

public class JobCreatedEvent(
    Guid jobId,
    string jobType,
    string targetResource,
    int priority,
    string? rateLimitKey)
    : BaseEvent
{
    public Guid JobId { get; } = jobId;
    public string JobType { get; } = jobType;
    public string TargetResource { get; } = targetResource;
    public int Priority { get; } = priority;
    public string? RateLimitKey { get; } = rateLimitKey;
    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;
}
