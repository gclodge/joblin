namespace Joblin.Domain.Events;

public class RateLimitConfigurationUpdatedEvent(
    Guid configurationId,
    int oldMaxConcurrentJobs,
    int newMaxConcurrentJobs,
    int oldTimeWindowSeconds,
    int newTimeWindowSeconds,
    int oldMaxJobsPerTimeWindow,
    int newMaxJobsPerTimeWindow)
    : BaseEvent
{
    public Guid ConfigurationId { get; } = configurationId;
    public int OldMaxConcurrentJobs { get; } = oldMaxConcurrentJobs;
    public int NewMaxConcurrentJobs { get; } = newMaxConcurrentJobs;
    public int OldTimeWindowSeconds { get; } = oldTimeWindowSeconds;
    public int NewTimeWindowSeconds { get; } = newTimeWindowSeconds;
    public int OldMaxJobsPerTimeWindow { get; } = oldMaxJobsPerTimeWindow;
    public int NewMaxJobsPerTimeWindow { get; } = newMaxJobsPerTimeWindow;
    public DateTimeOffset UpdatedAt { get; } = DateTimeOffset.UtcNow;
}
