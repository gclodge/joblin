namespace Joblin.Domain.ValueObjects;

/// <summary>
/// Value object representing current rate limiting metrics
/// </summary>
public class RateLimitMetrics(
    int currentActiveJobs,
    int maxConcurrentJobs,
    int jobsInCurrentWindow,
    int maxJobsPerWindow,
    DateTimeOffset windowStartTime,
    TimeSpan windowDuration,
    string rateLimitKey,
    string jobType) : ValueObject
{
    public int CurrentActiveJobs { get; } = currentActiveJobs;
    public int MaxConcurrentJobs { get; } = maxConcurrentJobs;
    public int JobsInCurrentWindow { get; } = jobsInCurrentWindow;
    public int MaxJobsPerWindow { get; } = maxJobsPerWindow;
    public DateTimeOffset WindowStartTime { get; } = windowStartTime;
    public TimeSpan WindowDuration { get; } = windowDuration;
    public string RateLimitKey { get; } = rateLimitKey ?? throw new ArgumentNullException(nameof(rateLimitKey));
    public string JobType { get; } = jobType ?? throw new ArgumentNullException(nameof(jobType));

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
    
    public static RateLimitMetrics Empty => new(
        currentActiveJobs: 0,
        maxConcurrentJobs: 0,
        jobsInCurrentWindow: 0,
        maxJobsPerWindow: 0,
        windowStartTime: DateTimeOffset.MinValue,
        windowDuration: TimeSpan.Zero,
        rateLimitKey: string.Empty,
        jobType: string.Empty);
}
