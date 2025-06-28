namespace Joblin.Domain.Events;

public class RateLimitConfigurationActivatedEvent(
    Guid configurationId,
    string name)
    : BaseEvent
{
    public Guid ConfigurationId { get; } = configurationId;
    public string Name { get; } = name;
    public DateTimeOffset ActivatedAt { get; } = DateTimeOffset.UtcNow;
}
