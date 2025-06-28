namespace Joblin.Domain.Events;

public class RateLimitConfigurationCreatedEvent(
    Guid configurationId,
    string name,
    int maxConcurrentJobs)
    : BaseEvent
{
    public Guid ConfigurationId { get; } = configurationId;
    public string Name { get; } = name;
    public int MaxConcurrentJobs { get; } = maxConcurrentJobs;
    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;
}
