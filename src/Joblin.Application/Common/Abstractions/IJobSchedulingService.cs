namespace Joblin.Application.Common.Abstractions;

public interface IJobSchedulingService
{
    /// <summary>
    /// Retrieves the next jobs to execute based on their status, rate limits, and scheduling.
    /// </summary>
    /// <param name="maxJobsToReturn"></param>
    /// <returns></returns>
    Task<IEnumerable<Job>> GetNextJobsToExecuteAsync(int maxJobsToReturn = 10);

    /// <summary>
    /// Groups jobs by their rate limit key for bulk operations.
    /// </summary>
    /// <param name="jobs"></param>
    /// <returns></returns>
    Dictionary<string, List<Job>> GroupJobsByRateLimitKey(
        IEnumerable<Job> jobs);
    
    /// <summary>
    /// Suggests an optimal scheduling time for a job based on its rate limit configuration and current state.
    /// </summary>
    /// <param name="job"></param>
    /// <param name="rateLimitConfig"></param>
    /// <param name="currentState"></param>
    /// <returns></returns>
    Task<DateTimeOffset> SuggestOptimalSchedulingTimeAsync(
        Job job,
        RateLimitConfiguration? rateLimitConfig = null,
        RateLimitState? currentState = null);
}