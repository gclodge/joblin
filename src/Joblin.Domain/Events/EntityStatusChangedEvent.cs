namespace Joblin.Domain.Events;

public class EntityStatusChangedEvent(
    Guid id,
    Status status,
    string? reason = null)
     : BaseEvent
{
    /// <summary>
    /// Gets the unique identifier of the entity whose status has changed.
    /// </summary>
    public Guid Id { get; } = id;

    /// <summary>
    /// Gets the new status of the entity.
    /// </summary>
    public Status Status { get; } = status;

    /// <summary>
    /// Gets an optional reason for the status change.
    /// </summary>
    public string? Reason { get; } = reason;
}
