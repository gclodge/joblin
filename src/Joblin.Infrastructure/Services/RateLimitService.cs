namespace Joblin.Infrastructure.Services;



/// <summary>
/// Domain service for handling rate limiting logic
/// </summary>
public class RateLimitService(
    IJoblinDbContext context,
    ILogger<RateLimitService> logger)
    : IRateLimitService
{
    private readonly IJoblinDbContext _context = context;
    private readonly ILogger<RateLimitService> _logger = logger;

    /// <summary>
    /// Determines the applicable rate limit configuration for a job
    /// </summary>
    public async Task<RateLimitConfiguration?> FindApplicableConfigurationAsync(
        JobExecutionContext context)
    {
        var configurations = await _context.RateLimitConfigurations
            .Where(config => config.IsActive)
            .ToListAsync();

        return configurations
            .Where(config => config.AppliesTo(context.JobType, context.GetEffectiveRateLimitKey()))
            .OrderByDescending(config => config.Priority)
            .ThenByDescending(config => GetSpecificityScore(config, context))
            .FirstOrDefault();
    }

    /// <summary>
    /// Checks if a job can proceed based on current rate limit state
    /// </summary>
    public async Task<RateLimitCheckResult> CheckRateLimit(
        JobExecutionContext context,
        RateLimitConfiguration? configuration,
        RateLimitState? currentState)
    {
        configuration ??= await FindApplicableConfigurationAsync(context);

        if (configuration == null)
        {
            return RateLimitCheckResult.NoConfigurationFound();
        }

        currentState = await _context.RateLimitStates
            .Where(s =>
                s.RateLimitKey == context.GetEffectiveRateLimitKey() &&
                s.JobType == context.JobType)
            .FirstOrDefaultAsync();

        // If no current state, assume we can start (first job for this key)
        if (currentState == null)
        {
            var newMetrics = new RateLimitMetrics(
                currentActiveJobs: 0,
                maxConcurrentJobs: configuration.MaxConcurrentJobs,
                jobsInCurrentWindow: 0,
                maxJobsPerWindow: configuration.MaxJobsPerTimeWindow,
                windowStartTime: DateTimeOffset.UtcNow,
                windowDuration: TimeSpan.FromSeconds(configuration.TimeWindowSeconds),
                rateLimitKey: context.GetEffectiveRateLimitKey(),
                jobType: context.JobType);

            return RateLimitCheckResult.Allowed(configuration, newMetrics);
        }

        // Check if job can start with current state
        if (currentState.CanStartJob(configuration))
        {
            var metrics = CreateMetrics(configuration, currentState, context);
            return RateLimitCheckResult.Allowed(configuration, metrics);
        }

        // Job cannot start - determine the reason and estimated wait time
        var denialMetrics = CreateMetrics(configuration, currentState, context);
        var denialReason = GetDenialReason(configuration, currentState);
        var estimatedWaitTime = CalculateEstimatedWaitTime(configuration, currentState);

        return RateLimitCheckResult.Denied(denialReason, configuration, denialMetrics, estimatedWaitTime);
    }

    /// <summary>
    /// Calculates a specificity score for configuration matching priority
    /// Higher score means more specific match
    /// </summary>
    private static int GetSpecificityScore(RateLimitConfiguration config, JobExecutionContext context)
    {
        var score = 0;

        // Specific job type is more specific than wildcard
        if (!string.IsNullOrEmpty(config.JobType))
        {
            score += 10;
        }

        // Specific key pattern is more specific than no pattern
        if (!string.IsNullOrEmpty(config.KeyPattern))
        {
            score += 5;

            // Exact match is more specific than wildcard
            if (!config.KeyPattern.Contains('*'))
            {
                score += 5;
            }
        }

        return score;
    }

    private static RateLimitMetrics CreateMetrics(
        RateLimitConfiguration configuration,
        RateLimitState currentState,
        JobExecutionContext context)
    {
        return new RateLimitMetrics(
            currentActiveJobs: currentState.ActiveJobCount,
            maxConcurrentJobs: configuration.MaxConcurrentJobs,
            jobsInCurrentWindow: currentState.JobsInCurrentWindow,
            maxJobsPerWindow: configuration.MaxJobsPerTimeWindow,
            windowStartTime: currentState.CurrentWindowStart,
            windowDuration: TimeSpan.FromSeconds(configuration.TimeWindowSeconds),
            rateLimitKey: context.GetEffectiveRateLimitKey(),
            jobType: context.JobType);
    }

    private static string GetDenialReason(RateLimitConfiguration configuration, RateLimitState currentState)
    {
        if (currentState.ActiveJobCount >= configuration.MaxConcurrentJobs)
        {
            return $"Concurrent job limit reached ({currentState.ActiveJobCount}/{configuration.MaxConcurrentJobs})";
        }

        if (currentState.JobsInCurrentWindow >= configuration.MaxJobsPerTimeWindow)
        {
            return $"Time window limit reached ({currentState.JobsInCurrentWindow}/{configuration.MaxJobsPerTimeWindow})";
        }

        return "Rate limit exceeded";
    }

    private static TimeSpan? CalculateEstimatedWaitTime(RateLimitConfiguration configuration, RateLimitState currentState)
    {
        // If concurrent limit is reached, we can't estimate when jobs will complete
        if (currentState.ActiveJobCount >= configuration.MaxConcurrentJobs)
        {
            return null; // Unknown wait time
        }

        // If time window limit is reached, wait until the window resets
        if (currentState.JobsInCurrentWindow >= configuration.MaxJobsPerTimeWindow)
        {
            var windowDuration = TimeSpan.FromSeconds(configuration.TimeWindowSeconds);
            var elapsed = DateTimeOffset.UtcNow - currentState.CurrentWindowStart;
            var remaining = windowDuration - elapsed;
            return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
        }

        return TimeSpan.Zero;
    }

    /// <summary>
    /// Checks rate limit for a specific key with provided configuration
    /// </summary>
    public async Task<RateLimitCheckResult> CheckRateLimitByKey(
        string key,
        RateLimitConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        var currentState = await _context.RateLimitStates
            .Where(s => s.RateLimitKey == key)
            .FirstOrDefaultAsync(cancellationToken);

        if (currentState == null)
        {
            var newMetrics = new RateLimitMetrics(
                currentActiveJobs: 0,
                maxConcurrentJobs: configuration.MaxConcurrentJobs,
                jobsInCurrentWindow: 0,
                maxJobsPerWindow: configuration.MaxJobsPerTimeWindow,
                windowStartTime: DateTimeOffset.UtcNow,
                windowDuration: TimeSpan.FromSeconds(configuration.TimeWindowSeconds),
                rateLimitKey: key,
                jobType: configuration.JobType ?? "");

            return RateLimitCheckResult.Allowed(configuration, newMetrics);
        }

        if (currentState.CanStartJob(configuration))
        {
            var metrics = new RateLimitMetrics(
                currentActiveJobs: currentState.ActiveJobCount,
                maxConcurrentJobs: configuration.MaxConcurrentJobs,
                jobsInCurrentWindow: currentState.JobsInCurrentWindow,
                maxJobsPerWindow: configuration.MaxJobsPerTimeWindow,
                windowStartTime: currentState.CurrentWindowStart,
                windowDuration: TimeSpan.FromSeconds(configuration.TimeWindowSeconds),
                rateLimitKey: key,
                jobType: configuration.JobType ?? "");

            return RateLimitCheckResult.Allowed(configuration, metrics);
        }

        var denialMetrics = new RateLimitMetrics(
            currentActiveJobs: currentState.ActiveJobCount,
            maxConcurrentJobs: configuration.MaxConcurrentJobs,
            jobsInCurrentWindow: currentState.JobsInCurrentWindow,
            maxJobsPerWindow: configuration.MaxJobsPerTimeWindow,
            windowStartTime: currentState.CurrentWindowStart,
            windowDuration: TimeSpan.FromSeconds(configuration.TimeWindowSeconds),
            rateLimitKey: key,
            jobType: configuration.JobType ?? "");

        var denialReason = GetDenialReason(configuration, currentState);
        var estimatedWaitTime = CalculateEstimatedWaitTime(configuration, currentState);

        return RateLimitCheckResult.Denied(denialReason, configuration, denialMetrics, estimatedWaitTime);
    }

    /// <summary>
    /// Checks if rate limit is exceeded for a meter identifier
    /// </summary>
    public async Task<bool> IsRateLimitExceededAsync(
        string meterIdentifier,
        TimeSpan window,
        int maxRequests,
        CancellationToken cancellationToken = default)
    {
        var windowStart = DateTimeOffset.UtcNow - window;
        
        var currentState = await _context.RateLimitStates
            .Where(s => s.RateLimitKey == meterIdentifier && s.CurrentWindowStart >= windowStart)
            .FirstOrDefaultAsync(cancellationToken);

        if (currentState == null)
        {
            return false; // No state means no requests yet
        }

        // Reset window if it's expired
        if (DateTimeOffset.UtcNow - currentState.CurrentWindowStart > window)
        {
            return false; // Window has expired, so not exceeded
        }

        return currentState.JobsInCurrentWindow >= maxRequests;
    }

    /// <summary>
    /// Increments the rate limit counter for a key
    /// </summary>
    public async Task IncrementRateLimitCounterAsync(
        string key,
        TimeSpan window,
        CancellationToken cancellationToken = default)
    {
        var existingState = await _context.RateLimitStates
            .Where(s => s.RateLimitKey == key)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingState == null)
        {
            //< Create new state
            var newState = new RateLimitState
            {
                RateLimitKey = key,
                JobType = "", // Generic for key-based operations
                ActiveJobCount = 0,
                JobsInCurrentWindow = 1,
                CurrentWindowStart = DateTimeOffset.UtcNow,
                LastUpdated = DateTimeOffset.UtcNow
            };

            _context.RateLimitStates.Add(newState);
        }
        else
        {
            //< Check if we need to reset the window
            if (DateTimeOffset.UtcNow - existingState.CurrentWindowStart > window)
            {
                existingState.CurrentWindowStart = DateTimeOffset.UtcNow;
                existingState.JobsInCurrentWindow = 1;
            }
            else
            {
                existingState.JobsInCurrentWindow++;
            }
            
            existingState.LastUpdated = DateTimeOffset.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
