namespace Joblin.Domain.Enums;

/// <summary>
/// Represents the status of a job in within the 'joblin' system.
/// </summary>
public enum Status
{
    /// <summary>
    /// This status indicates that the job is queued and waiting to be processed.
    /// </summary>
    Queued = 0,
    /// <summary>
    /// This status indicates that the job is currently being processed & has not yet completed.
    /// </summary>
    InProgress = 1,
    /// <summary>
    /// This status indicates that the job has been successfully completed.
    /// </summary>
    Completed = 2,
    /// <summary>
    /// This status indicates that the job has failed to complete successfully.
    /// </summary>
    Failed = 3,
    /// <summary>
    /// This status indicates that the job has been cancelled and will not be processed further.
    /// </summary>
    Cancelled = 4
}
