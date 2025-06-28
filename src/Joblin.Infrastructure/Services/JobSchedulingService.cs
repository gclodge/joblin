namespace Joblin.Infrastructure.Services;

/// <summary>
/// Domain service for job scheduling and prioritization
/// </summary>
public class JobSchedulingService(
    IJoblinDbContext context,
    IRateLimitService rateLimitService)
    : IJobSchedulingService
{
    private readonly IJoblinDbContext _context = context;
    private readonly IRateLimitService _rateLimitService = rateLimitService;

    public async Task<IEnumerable<Job>> GetNextJobsToExecuteAsync(int maxJobsToReturn = 10)
    {
        var queuedJobs = await _context.Jobs
            .Where(j => j.Status == Status.Queued &&
                        (!j.ScheduledFor.HasValue || j.ScheduledFor.Value <= DateTimeOffset.UtcNow))
            .ToListAsync();

        var eligibleJobs = new List<(Job Job, int Priority)>();

        foreach (var job in queuedJobs)
        {
            var context = new JobExecutionContext(job.JobType, job.TargetResource, job.RateLimitKey);
            var rateLimitCheck = await _rateLimitService.CheckRateLimit(context, null, null);

            if (rateLimitCheck.CanProceed)
            {
                var effectivePriority = CalculateEffectivePriority(job);
                eligibleJobs.Add((job, effectivePriority));
            }
        }

        return eligibleJobs
            .OrderByDescending(x => x.Priority)
            .Take(maxJobsToReturn)
            .Select(x => x.Job);
    }

    /// <summary>
    /// Calculates effective priority considering job age and base priority
    /// </summary>
    private static int CalculateEffectivePriority(Job job)
    {
        var basePriority = job.Priority;

        // Add age bonus - older jobs get higher priority to prevent starvation
        var ageInHours = (DateTimeOffset.UtcNow - job.Created).TotalHours;
        var agePriority = (int)(ageInHours * 10); // 10 priority points per hour

        // Add retry penalty - failed jobs get slightly lower priority
        var retryPenalty = job.RetryCount * 5;

        return basePriority + agePriority - retryPenalty;
    }

    /// <summary>
    /// Groups jobs by their rate limit key for bulk operations
    /// </summary>
    public Dictionary<string, List<Job>> GroupJobsByRateLimitKey(IEnumerable<Job> jobs)
    {
        var groups = new Dictionary<string, List<Job>>();

        foreach (var job in jobs)
        {
            var context = new JobExecutionContext(job.JobType, job.TargetResource, job.RateLimitKey);
            var key = context.GetEffectiveRateLimitKey();

            if (!groups.TryGetValue(key, out List<Job>? value))
            {
                value = [];
                groups[key] = value;
            }

            value.Add(job);
        }

        return groups;
    }

    /// <summary>
    /// Suggests optimal scheduling time for a job based on rate limits
    /// </summary>
    public async Task<DateTimeOffset> SuggestOptimalSchedulingTimeAsync(
        Job job,
        RateLimitConfiguration? rateLimitConfig = null,
        RateLimitState? currentState = null)
    {
        var baseTime = DateTimeOffset.UtcNow;
        var context = new JobExecutionContext(job.JobType, job.TargetResource, job.RateLimitKey);

        //< Get rate limit configuration if not provided
        rateLimitConfig ??= await _rateLimitService.FindApplicableConfigurationAsync(context);

        //< If no rate limiting, can schedule immediately
        if (rateLimitConfig == null)
        {
            return baseTime;
        }

        //< Check current rate limit status
        var rateLimitCheck = await _rateLimitService.CheckRateLimit(context, rateLimitConfig, currentState);

        if (rateLimitCheck.CanProceed)
        {
            return baseTime;
        }

        //< If denied, use the estimated wait time from the check result
        if (rateLimitCheck.EstimatedWaitTime.HasValue)
        {
            return baseTime.Add(rateLimitCheck.EstimatedWaitTime.Value);
        }

        //< Fallback estimation
        return baseTime.AddMinutes(5);
    }
}