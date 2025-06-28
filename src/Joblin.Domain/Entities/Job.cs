namespace Joblin.Domain.Entities;

/// <summary>
/// Represents an asynchronous job in the system
/// </summary>
public class Job : BaseStatefulEntity
{
    /// <summary>
    /// The type of job (e.g., "MeterDataPull", "FileProcessing", etc.)
    /// </summary>
    public string JobType { get; private set; } = default!;

    /// <summary>
    /// The target resource identifier (e.g., device ID, file path, etc.)
    /// </summary>
    public string TargetResource { get; private set; } = default!;

    /// <summary>
    /// The priority of the job (higher number = higher priority)
    /// </summary>
    public int Priority { get; private set; }

    /// <summary>
    /// When the job should be executed (for scheduled jobs)
    /// </summary>
    public DateTimeOffset? ScheduledFor { get; private set; }

    /// <summary>
    /// When the job was actually started
    /// </summary>
    public DateTimeOffset? StartedAt { get; private set; }

    /// <summary>
    /// When the job was completed (successfully or failed)
    /// </summary>
    public DateTimeOffset? CompletedAt { get; private set; }

    /// <summary>
    /// The external job ID from the execution engine (e.g., Durable Functions instance ID)
    /// </summary>
    public string? ExternalJobId { get; private set; }

    /// <summary>
    /// JSON payload for the job
    /// </summary>
    public string? Payload { get; private set; }

    /// <summary>
    /// Result data from job execution
    /// </summary>
    public string? Result { get; private set; }

    /// <summary>
    /// Error message if the job failed
    /// </summary>
    public string? ErrorMessage { get; private set; }

    /// <summary>
    /// Number of retry attempts made
    /// </summary>
    public int RetryCount { get; private set; }

    /// <summary>
    /// Maximum number of retries allowed
    /// </summary>
    public int MaxRetries { get; private set; }

    /// <summary>
    /// Rate limit key for grouping jobs (e.g., device ID for device-specific rate limiting)
    /// </summary>
    public string? RateLimitKey { get; private set; }

    /// <summary>
    /// Navigation property to rate limit configuration
    /// </summary>
    public RateLimitConfiguration? RateLimitConfiguration { get; private set; }

    /// <summary>
    /// Foreign key to rate limit configuration
    /// </summary>
    public Guid? RateLimitConfigurationId { get; private set; }

    public void Start(string? externalJobId = null)
    {
        if (Status != Status.Queued) throw new InvalidOperationException($"Cannot start job in {Status} status");

        StartedAt = DateTimeOffset.UtcNow;
        ExternalJobId = externalJobId;
        SetStatus(Status.InProgress, "Job started");
        
        AddDomainEvent(new JobStartedEvent(Id, ExternalJobId, StartedAt.Value));
    }

    public void Complete(string? result = null)
    {
        if (Status != Status.InProgress)
            throw new InvalidOperationException($"Cannot complete job in {Status} status");

        CompletedAt = DateTimeOffset.UtcNow;
        Result = result;
        SetStatus(Status.Completed, "Job completed successfully");
        
        AddDomainEvent(new JobCompletedEvent(Id, CompletedAt.Value, result));
    }

    public void Fail(string errorMessage, bool shouldRetry = true)
    {
        if (Status != Status.InProgress)
            throw new InvalidOperationException($"Cannot fail job in {Status} status");

        ErrorMessage = errorMessage;
        CompletedAt = DateTimeOffset.UtcNow;

        if (shouldRetry && RetryCount < MaxRetries)
        {
            RetryCount++;
            SetStatus(Status.Queued, $"Job failed, retry {RetryCount}/{MaxRetries}");
            AddDomainEvent(new JobRetryScheduledEvent(Id, RetryCount, MaxRetries, errorMessage));
        }
        else
        {
            SetStatus(Status.Failed, "Job failed - max retries exceeded");
            AddDomainEvent(new JobFailedEvent(Id, errorMessage, RetryCount));
        }
    }

    public void UpdateExternalJobId(string externalJobId)
    {
        ExternalJobId = externalJobId;
    }

    public void Reschedule(DateTimeOffset newScheduledTime, string? reason = null)
    {
        if (Status != Status.Queued)
            throw new InvalidOperationException($"Cannot reschedule job in {Status} status");

        ScheduledFor = newScheduledTime;
        AddDomainEvent(new JobRescheduledEvent(Id, newScheduledTime, reason));
    }

    protected override void OnCanceled(string? reason)
    {
        CompletedAt = DateTimeOffset.UtcNow;
        base.OnCanceled(reason);
    }
}
