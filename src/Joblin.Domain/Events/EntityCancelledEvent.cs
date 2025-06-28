using System;

namespace Joblin.Domain.Events;

public class EntityCancelledEvent<T>(
    Guid entityId,
    string? reason)
    : BaseEvent
    where T : class
{
    public Guid EntityId { get; } = entityId;
    public string? Reason { get; } = reason;
    public string EntityType { get; } = typeof(T).Name;
}
