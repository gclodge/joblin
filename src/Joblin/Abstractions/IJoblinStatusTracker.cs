using Joblin.Models;

namespace Joblin.Abstractions;

public interface IJoblinStatusTracker
{
    /// <summary>
    /// Updates the status of a job.
    /// </summary>
    /// <param name="jobId"></param>
    /// <param name="status"></param>
    /// <param name="result"></param>
    /// <param name="errorMessage"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UpdateStatusAsync(string jobId, JobStatus status, object? result = null, string? errorMessage = null, CancellationToken cancellationToken = default);
   
    /// <summary>
    /// Gets the status of a job by its ID.
    /// </summary>
    /// <param name="jobId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<JobStatus> GetStatusAsync(string jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves detailed information about a job by its ID.
    /// </summary>
    /// <param name="jobId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<JobDetail?> GetJobAsync(string jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of jobs based on the provided filter.
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<JobSummary>> GetJobsAsync(JobFilter? filter = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Records a heartbeat for a job, updating its progress and message.
    /// </summary>
    /// <param name="jobId"></param>
    /// <param name="progress"></param>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RecordHeartbeatAsync(string jobId, int? progress = null, string? message = null, CancellationToken cancellationToken = default);
}
