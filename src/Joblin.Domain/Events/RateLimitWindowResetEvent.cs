namespace Joblin.Domain.Events;

public class RateLimitWindowResetEvent(
    string rateLimitKey,
    string jobType,
    DateTimeOffset newWindowStart)
    : BaseEvent
{
    public string RateLimitKey { get; } = rateLimitKey;
    public string JobType { get; } = jobType;
    public DateTimeOffset NewWindowStart { get; } = newWindowStart;
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
