namespace Joblin.Domain.Entities;

/// <summary>
/// Tracks the current state of rate limiting for a specific key
/// This is a runtime entity used for tracking active jobs and rate limit state
/// </summary>
public class RateLimitState : BaseEntity
{
    /// <summary>
    /// The rate limit key this state tracks
    /// </summary>
    public required string RateLimitKey { get; set; }

    /// <summary>
    /// The job type this state applies to
    /// </summary>
    public required string JobType { get; set; } 

    /// <summary>
    /// Number of currently active (in-progress) jobs for this key
    /// </summary>
    public int ActiveJobCount { get; set; }

    /// <summary>
    /// Number of jobs started within the current time window
    /// </summary>
    public int JobsInCurrentWindow { get; set; }

    /// <summary>
    /// Start of the current time window
    /// </summary>
    public DateTimeOffset CurrentWindowStart { get; set; }

    /// <summary>
    /// Foreign key to the rate limit configuration being applied
    /// </summary>
    public Guid RateLimitConfigurationId { get; set; }

    /// <summary>
    /// Navigation property to rate limit configuration
    /// </summary>
    public RateLimitConfiguration RateLimitConfiguration { get; set; } = default!;

    /// <summary>
    /// Last time this state was updated
    /// </summary>
    public DateTimeOffset LastUpdated { get; set; }

    /// <summary>
    /// Checks if a new job can be started based on rate limits
    /// </summary>
    public bool CanStartJob(RateLimitConfiguration config)
    {
        UpdateTimeWindow(config);
        
        //< Check concurrent job limit
        if (ActiveJobCount >= config.MaxConcurrentJobs) return false;

        //< Check time window limit
        if (JobsInCurrentWindow >= config.MaxJobsPerTimeWindow) return false;

        return true;
    }

    /// <summary>
    /// Records that a job has started
    /// </summary>
    public void JobStarted(RateLimitConfiguration config)
    {
        UpdateTimeWindow(config);
        
        ActiveJobCount++;
        JobsInCurrentWindow++;
        LastUpdated = DateTimeOffset.UtcNow;

        AddDomainEvent(new RateLimitJobStartedEvent(
            RateLimitKey, 
            JobType, 
            ActiveJobCount, 
            JobsInCurrentWindow,
            config.MaxConcurrentJobs,
            config.MaxJobsPerTimeWindow));
    }

    /// <summary>
    /// Records that a job has completed (successfully or failed)
    /// </summary>
    public void JobCompleted()
    {
        if (ActiveJobCount > 0)
        {
            ActiveJobCount--;
            LastUpdated = DateTimeOffset.UtcNow;

            AddDomainEvent(new RateLimitJobCompletedEvent(
                RateLimitKey, 
                JobType, 
                ActiveJobCount));
        }
    }

    /// <summary>
    /// Updates the time window if needed
    /// </summary>
    private void UpdateTimeWindow(RateLimitConfiguration config)
    {
        var now = DateTimeOffset.UtcNow;
        var windowDuration = TimeSpan.FromSeconds(config.TimeWindowSeconds);
        
        if (now - CurrentWindowStart >= windowDuration)
        {
            //< Start a new time window
            CurrentWindowStart = now;
            JobsInCurrentWindow = 0;
            LastUpdated = now;

            AddDomainEvent(new RateLimitWindowResetEvent(
                RateLimitKey, 
                JobType, 
                CurrentWindowStart));
        }
    }

    /// <summary>
    /// Resets the rate limit state (useful for configuration changes)
    /// </summary>
    public void Reset()
    {
        ActiveJobCount = 0;
        JobsInCurrentWindow = 0;
        CurrentWindowStart = DateTimeOffset.UtcNow;
        LastUpdated = DateTimeOffset.UtcNow;

        AddDomainEvent(new RateLimitStateResetEvent(RateLimitKey, JobType));
    }
}
