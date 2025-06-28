namespace Joblin.Domain.Abstractions;

public interface IBaseStatefulEntity
{
    Status Status { get; }

    void Cancel(string? reason);
    void SetStatus(Status status, string? reason);
}
