namespace Joblin.Domain.Events;

public class RateLimitConfigurationDeactivatedEvent(
    Guid configurationId,
    string name)
    : BaseEvent
{
    public Guid ConfigurationId { get; } = configurationId;
    public string Name { get; } = name;
    public DateTimeOffset DeactivatedAt { get; } = DateTimeOffset.UtcNow;
}
