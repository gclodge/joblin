namespace Joblin.Domain.Entities;

/// <summary>
/// Represents rate limiting configuration for different job types or resources
/// </summary>
public class RateLimitConfiguration : BaseAuditableEntity
{
    /// <summary>
    /// The name/identifier for this rate limit configuration
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Description of what this rate limit is for
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Maximum number of concurrent jobs allowed
    /// </summary>
    public int MaxConcurrentJobs { get; set; }

    /// <summary>
    /// Time window for rate limiting (in seconds)
    /// </summary>
    public int TimeWindowSeconds { get; set; }

    /// <summary>
    /// Maximum number of jobs allowed within the time window
    /// </summary>
    public int MaxJobsPerTimeWindow { get; set; }

    /// <summary>
    /// Whether this configuration is currently active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Job type this configuration applies to (null = applies to all types)
    /// </summary>
    public string? JobType { get; set; }

    /// <summary>
    /// Pattern for matching rate limit keys (supports wildcards)
    /// </summary>
    public string? KeyPattern { get; set; }

    /// <summary>
    /// Priority of this configuration (higher number = higher priority when multiple configs match)
    /// </summary>
    public int Priority { get; set; }

    public void UpdateLimits(int maxConcurrentJobs, int timeWindowSeconds, int maxJobsPerTimeWindow)
    {
        if (maxConcurrentJobs <= 0)
            throw new ArgumentException("Max concurrent jobs must be greater than 0", nameof(maxConcurrentJobs));
        
        if (timeWindowSeconds <= 0)
            throw new ArgumentException("Time window must be greater than 0", nameof(timeWindowSeconds));
        
        if (maxJobsPerTimeWindow <= 0)
            throw new ArgumentException("Max jobs per time window must be greater than 0", nameof(maxJobsPerTimeWindow));

        var oldMaxConcurrent = MaxConcurrentJobs;
        var oldTimeWindow = TimeWindowSeconds;
        var oldMaxPerWindow = MaxJobsPerTimeWindow;

        MaxConcurrentJobs = maxConcurrentJobs;
        TimeWindowSeconds = timeWindowSeconds;
        MaxJobsPerTimeWindow = maxJobsPerTimeWindow;

        AddDomainEvent(new RateLimitConfigurationUpdatedEvent(
            Id, 
            oldMaxConcurrent, 
            maxConcurrentJobs,
            oldTimeWindow,
            timeWindowSeconds,
            oldMaxPerWindow,
            maxJobsPerTimeWindow));
    }

    public void Activate()
    {
        if (IsActive) return;
        
        IsActive = true;
        AddDomainEvent(new RateLimitConfigurationActivatedEvent(Id, Name));
    }

    public void Deactivate()
    {
        if (!IsActive) return;
        
        IsActive = false;
        AddDomainEvent(new RateLimitConfigurationDeactivatedEvent(Id, Name));
    }

    public void UpdateJobTypeFilter(string? jobType)
    {
        JobType = jobType;
        AddDomainEvent(new RateLimitConfigurationUpdatedEvent(Id, MaxConcurrentJobs, MaxConcurrentJobs, TimeWindowSeconds, TimeWindowSeconds, MaxJobsPerTimeWindow, MaxJobsPerTimeWindow));
    }

    public void UpdateKeyPattern(string? keyPattern)
    {
        KeyPattern = keyPattern;
        AddDomainEvent(new RateLimitConfigurationUpdatedEvent(Id, MaxConcurrentJobs, MaxConcurrentJobs, TimeWindowSeconds, TimeWindowSeconds, MaxJobsPerTimeWindow, MaxJobsPerTimeWindow));
    }

    /// <summary>
    /// Checks if this configuration applies to the given job type and rate limit key
    /// </summary>
    public bool AppliesTo(string jobType, string? rateLimitKey)
    {
        if (!IsActive) return false;

        //< Check job type filter
        if (JobType != null && !string.Equals(JobType, jobType, StringComparison.OrdinalIgnoreCase))
            return false;

        //< Check key pattern
        if (!string.IsNullOrEmpty(KeyPattern) && !string.IsNullOrEmpty(rateLimitKey))
        {
            //< Simple wildcard matching - could be enhanced with regex if needed
            if (KeyPattern.Contains('*'))
            {
                var pattern = KeyPattern.Replace("*", ".*");
                return System.Text.RegularExpressions.Regex.IsMatch(rateLimitKey, pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }
            else
            {
                return string.Equals(KeyPattern, rateLimitKey, StringComparison.OrdinalIgnoreCase);
            }
        }

        return true;
    }
}
