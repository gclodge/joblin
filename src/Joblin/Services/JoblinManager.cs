namespace Joblin.Services;

public class JoblinManager : IJoblinManager
{
    private readonly IJoblinQueue _jobQueue;
    private readonly IJoblinStatusTracker _statusTracker;

    public JoblinManager(
        IJoblinQueue jobQueue,
        IJoblinStatusTracker statusTracker)
    {
        _jobQueue = jobQueue ?? throw new ArgumentNullException(nameof(jobQueue));
        _statusTracker = statusTracker ?? throw new ArgumentNullException(nameof(statusTracker));
    }

    public async Task<string> SubmitJobAsync<T>(T jobData, string webhookUrl, JobOptions? options = null, CancellationToken cancellationToken = default)
    {
        var jobId = Guid.NewGuid().ToString();
        
        var job = new JobSubmission
        {
            Id = jobId,
            Data = jobData,
            WebhookUrl = webhookUrl,
            SubmittedAt = DateTime.UtcNow,
            Status = JobStatus.Queued,
            Options = options ?? new JobOptions()
        };

        await _statusTracker.UpdateStatusAsync(jobId, JobStatus.Queued, cancellationToken: cancellationToken);
        await _jobQueue.EnqueueAsync(job, cancellationToken);
        
        return jobId;
    }

    public async Task<JobStatus> GetJobStatusAsync(string jobId, CancellationToken cancellationToken = default)
    {
        return await _statusTracker.GetStatusAsync(jobId, cancellationToken);
    }

    public async Task<IEnumerable<JobSummary>> GetJobsAsync(JobFilter? filter = null, CancellationToken cancellationToken = default)
    {
        return await _statusTracker.GetJobsAsync(filter, cancellationToken);
    }

    public async Task UpdateJobStatusAsync(string jobId, JobStatus status, object? result = null, string? errorMessage = null, CancellationToken cancellationToken = default)
    {
        await _statusTracker.UpdateStatusAsync(jobId, status, result, errorMessage, cancellationToken);
    }

    public async Task RecordHeartbeatAsync(string jobId, int? progress = null, string? message = null, CancellationToken cancellationToken = default)
    {
        await _statusTracker.RecordHeartbeatAsync(jobId, progress, message, cancellationToken);
    }
}
