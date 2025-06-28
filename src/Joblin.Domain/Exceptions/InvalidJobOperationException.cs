namespace Joblin.Domain.Exceptions;

/// <summary>
/// Exception thrown when a job operation is invalid based on current state
/// </summary>
public class InvalidJobOperationException : DomainException
{
    public Guid JobId { get; }
    public Status CurrentStatus { get; }
    public string Operation { get; }

    public InvalidJobOperationException(
        Guid jobId, 
        Status currentStatus, 
        string operation, 
        string message) 
        : base(message)
    {
        JobId = jobId;
        CurrentStatus = currentStatus;
        Operation = operation;
    }

    public InvalidJobOperationException(
        Guid jobId, 
        Status currentStatus, 
        string operation, 
        string message, 
        Exception innerException) 
        : base(message, innerException)
    {
        JobId = jobId;
        CurrentStatus = currentStatus;
        Operation = operation;
    }
}
