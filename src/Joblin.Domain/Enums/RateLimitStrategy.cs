namespace Joblin.Domain.Enums;

/// <summary>
/// Represents different strategies for handling rate limit violations
/// </summary>
public enum RateLimitStrategy
{
    /// <summary>
    /// Queue the job and wait for rate limits to allow execution
    /// </summary>
    Queue = 0,
    
    /// <summary>
    /// Reject the job immediately if rate limits are exceeded
    /// </summary>
    Reject = 1,
    
    /// <summary>
    /// Delay the job execution to a future time when rate limits allow
    /// </summary>
    Delay = 2,
    
    /// <summary>
    /// Use a different rate limit pool or bypass rate limits
    /// </summary>
    Bypass = 3
}
