namespace Joblin.Domain.Events;

public class JobRescheduledEvent(
    Guid jobId,
    DateTimeOffset newScheduledTime,
    string? reason)
    : BaseEvent
{
    public Guid JobId { get; } = jobId;
    public DateTimeOffset NewScheduledTime { get; } = newScheduledTime;
    public string? Reason { get; } = reason;
    public DateTimeOffset RescheduledAt { get; } = DateTimeOffset.UtcNow;
}
