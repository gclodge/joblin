namespace Joblin.Application.Common.Abstractions;

public interface IRateLimitService
{
    /// <summary>
    /// Checks if the rate limit for a job can be exceeded based on the current context and configuration.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="configuration"></param>
    /// <param name="currentState"></param>
    /// <returns></returns>
    Task<RateLimitCheckResult> CheckRateLimit(
        JobExecutionContext context,
        RateLimitConfiguration? configuration,
        RateLimitState? currentState);

    /// <summary>
    /// Checks if the rate limit for a job can be exceeded based on a specific key and configuration.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="configuration"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<RateLimitCheckResult> CheckRateLimitByKey(
        string key,
        RateLimitConfiguration configuration,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    Task<RateLimitConfiguration?> FindApplicableConfigurationAsync(
        JobExecutionContext context);

    /// <summary>
    /// Checks if the rate limit for a specific entity identifier is exceeded.
    /// </summary>
    /// <param name="entityIdentifier"></param>
    /// <param name="window"></param>
    /// <param name="maxRequests"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> IsRateLimitExceededAsync(
        string entityIdentifier,
        TimeSpan window,
        int maxRequests,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Increments the rate limit counter for a specific key within a defined time window.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="window"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task IncrementRateLimitCounterAsync(
        string key,
        TimeSpan window,
        CancellationToken cancellationToken = default);
}