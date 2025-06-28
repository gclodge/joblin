namespace Joblin.Domain.Exceptions;

/// <summary>
/// Exception thrown when rate limit constraints are violated
/// </summary>
public class RateLimitExceededException : DomainException
{
    public string RateLimitKey { get; }
    public string JobType { get; }
    public RateLimitMetrics? Metrics { get; }
    public TimeSpan? EstimatedWaitTime { get; }

    public RateLimitExceededException(
        string rateLimitKey,
        string jobType,
        string message,
        RateLimitMetrics? metrics = null,
        TimeSpan? estimatedWaitTime = null) 
        : base(message)
    {
        RateLimitKey = rateLimitKey;
        JobType = jobType;
        Metrics = metrics;
        EstimatedWaitTime = estimatedWaitTime;
    }

    public RateLimitExceededException(
        string rateLimitKey,
        string jobType,
        string message,
        Exception innerException,
        RateLimitMetrics? metrics = null,
        TimeSpan? estimatedWaitTime = null) 
        : base(message, innerException)
    {
        RateLimitKey = rateLimitKey;
        JobType = jobType;
        Metrics = metrics;
        EstimatedWaitTime = estimatedWaitTime;
    }
}
