namespace Joblin.Domain.Events;

public class RateLimitStateResetEvent(
    string rateLimitKey,
    string jobType)
    : BaseEvent
{
    public string RateLimitKey { get; } = rateLimitKey;
    public string JobType { get; } = jobType;
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
