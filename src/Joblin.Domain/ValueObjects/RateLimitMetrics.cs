namespace Joblin.Domain.ValueObjects;

/// <summary>
/// Value object representing current rate limiting metrics
/// </summary>
public class RateLimitMetrics : ValueObject
{
    public int CurrentActiveJobs { get; init; }
    public int MaxConcurrentJobs { get; init; } 
    public int JobsInCurrentWindow { get; init; }
    public int MaxJobsPerWindow { get; init; } 
    public DateTimeOffset WindowStartTime { get; init; }
    public TimeSpan WindowDuration { get; init; } 
    public string RateLimitKey { get; init; } = string.Empty;
    public string JobType { get; init; } = string.Empty;

    /// <summary>
    /// Percentage of concurrent job capacity being used
    /// </summary>
    public double ConcurrentJobUtilization => MaxConcurrentJobs > 0 ?
        (double)CurrentActiveJobs / MaxConcurrentJobs * 100 : 0;

    /// <summary>
    /// Percentage of time window capacity being used
    /// </summary>
    public double WindowUtilization => MaxJobsPerWindow > 0 ?
        (double)JobsInCurrentWindow / MaxJobsPerWindow * 100 : 0;

    /// <summary>
    /// Time remaining in the current window
    /// </summary>
    public TimeSpan TimeRemainingInWindow
    {
        get
        {
            var elapsed = DateTimeOffset.UtcNow - WindowStartTime;
            var remaining = WindowDuration - elapsed;
            return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
        }
    }

    /// <summary>
    /// Whether the concurrent job limit is reached
    /// </summary>
    public bool IsConcurrentLimitReached => CurrentActiveJobs >= MaxConcurrentJobs;

    /// <summary>
    /// Whether the time window limit is reached
    /// </summary>
    public bool IsWindowLimitReached => JobsInCurrentWindow >= MaxJobsPerWindow;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return CurrentActiveJobs;
        yield return MaxConcurrentJobs;
        yield return JobsInCurrentWindow;
        yield return MaxJobsPerWindow;
        yield return WindowStartTime;
        yield return WindowDuration;
        yield return RateLimitKey;
        yield return JobType;
    }
    
    public static RateLimitMetrics Empty => new RateLimitMetrics
    {
        CurrentActiveJobs = 0,
        MaxConcurrentJobs = 0,
        JobsInCurrentWindow = 0,
        MaxJobsPerWindow = 0,
        WindowStartTime = DateTimeOffset.MinValue,
        WindowDuration = TimeSpan.Zero,
        RateLimitKey = string.Empty,
        JobType = string.Empty
    };
}
