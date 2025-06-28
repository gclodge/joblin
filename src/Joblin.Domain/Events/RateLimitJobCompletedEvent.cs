namespace Joblin.Domain.Events;

public class RateLimitJobCompletedEvent(
    string rateLimitKey,
    string jobType,
    int activeJobCount)
    : BaseEvent
{
    public string RateLimitKey { get; } = rateLimitKey;
    public string JobType { get; } = jobType;
    public int ActiveJobCount { get; } = activeJobCount;
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
