namespace Joblin.Domain.Events;

public class JobCompletedEvent(
    Guid jobId,
    DateTimeOffset completedAt,
    string? result)
    : BaseEvent
{
    public Guid JobId { get; } = jobId;
    public DateTimeOffset CompletedAt { get; } = completedAt;
    public string? Result { get; } = result;
}
