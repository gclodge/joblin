namespace Joblin.Domain.Abstractions;

public abstract class BaseStatefulEntity : BaseAuditableEntity, IHasStatus, IBaseStatefulEntity
{
    public Status Status { get; set; } = Status.Queued;

    public virtual void SetStatus(Status status, string? reason)
    {
        if (Status == status) return;

        Status = status;
        OnStatusChanged(status, reason);
    }

    protected virtual void OnStatusChanged(Status status, string? reason)
    {
        this.AddDomainEvent(new EntityStatusChangedEvent(Id, status, reason));
    }

    public virtual void Cancel(string? reason)
    {
        SetStatus(Status.Cancelled, reason);
        OnCanceled(reason);
    }

    protected virtual void OnCanceled(string? reason)
    {
        this.AddDomainEvent(new EntityCancelledEvent<BaseStatefulEntity>(Id, reason));
    }
}