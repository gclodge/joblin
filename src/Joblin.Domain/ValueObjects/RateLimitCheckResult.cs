namespace Joblin.Domain.ValueObjects;

/// <summary>
/// Value object representing the result of a rate limit check
/// </summary>
public class RateLimitCheckResult : ValueObject
{
    public bool CanProceed { get; }
    public string? DenialReason { get; }
    public RateLimitConfiguration? AppliedConfiguration { get; }
    public TimeSpan? EstimatedWaitTime { get; }
    public RateLimitMetrics? Metrics { get; }

    private RateLimitCheckResult(
        bool canProceed, 
        string? denialReason = null,
        RateLimitConfiguration? appliedConfiguration = null,
        TimeSpan? estimatedWaitTime = null,
        RateLimitMetrics? metrics = null)
    {
        CanProceed = canProceed;
        DenialReason = denialReason;
        AppliedConfiguration = appliedConfiguration;
        EstimatedWaitTime = estimatedWaitTime;
        Metrics = metrics;
    }

    public static RateLimitCheckResult Allowed(
        RateLimitConfiguration appliedConfiguration,
        RateLimitMetrics metrics)
    {
        return new RateLimitCheckResult(
            canProceed: true,
            appliedConfiguration: appliedConfiguration,
            metrics: metrics);
    }

    public static RateLimitCheckResult Denied(
        string reason,
        RateLimitConfiguration appliedConfiguration,
        RateLimitMetrics metrics,
        TimeSpan? estimatedWaitTime = null)
    {
        return new RateLimitCheckResult(
            canProceed: false,
            denialReason: reason,
            appliedConfiguration: appliedConfiguration,
            estimatedWaitTime: estimatedWaitTime,
            metrics: metrics);
    }

    public static RateLimitCheckResult NoConfigurationFound()
    {
        return new RateLimitCheckResult(
            canProceed: true,
            denialReason: "No rate limit configuration found - allowing by default");
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return CanProceed;
        yield return DenialReason;
        yield return AppliedConfiguration?.Id;
        yield return EstimatedWaitTime;
        yield return Metrics;
    }
}
