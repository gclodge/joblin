namespace Joblin.Domain.Events;

public class JobRetryScheduledEvent(
    Guid jobId,
    int retryCount,
    int maxRetries,
    string errorMessage)
    : BaseEvent
{
    public Guid JobId { get; } = jobId;
    public int RetryCount { get; } = retryCount;
    public int MaxRetries { get; } = maxRetries;
    public string ErrorMessage { get; } = errorMessage;
    public DateTimeOffset ScheduledAt { get; } = DateTimeOffset.UtcNow;
}
