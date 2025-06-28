namespace Joblin.Domain.Events;

public class JobFailedEvent(
    Guid jobId,
    string errorMessage,
    int retryCount)
    : BaseEvent
{
    public Guid JobId { get; } = jobId;
    public string ErrorMessage { get; } = errorMessage;
    public int RetryCount { get; } = retryCount;
    public DateTimeOffset FailedAt { get; } = DateTimeOffset.UtcNow;
}
