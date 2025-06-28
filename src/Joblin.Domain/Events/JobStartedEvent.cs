namespace Joblin.Domain.Events;

public class JobStartedEvent(
    Guid jobId,
    string? externalJobId,
    DateTimeOffset startedAt)
    : BaseEvent
{
    public Guid JobId { get; } = jobId;
    public string? ExternalJobId { get; } = externalJobId;
    public DateTimeOffset StartedAt { get; } = startedAt;
}
