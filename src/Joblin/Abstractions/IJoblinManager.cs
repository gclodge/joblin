using Joblin.Models;

namespace Joblin.Abstractions;

public interface IJoblinManager
{
    Task<string> SubmitJobAsync<T>(T jobData, string webhookUrl, JobOptions? options = null, CancellationToken cancellationToken = default);
    Task<JobStatus> GetJobStatusAsync(string jobId, CancellationToken cancellationToken = default);
    Task<IEnumerable<JobSummary>> GetJobsAsync(JobFilter? filter = null, CancellationToken cancellationToken = default);
    Task UpdateJobStatusAsync(string jobId, JobStatus status, object? result = null, string? errorMessage = null, CancellationToken cancellationToken = default);
    Task RecordHeartbeatAsync(string jobId, int? progress = null, string? message = null, CancellationToken cancellationToken = default);
}
