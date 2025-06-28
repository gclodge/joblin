namespace Joblin.Domain.ValueObjects;

/// <summary>
/// Value object representing job execution context
/// </summary>
public class JobExecutionContext : ValueObject
{
    public string JobType { get; }
    public string TargetResource { get; }
    public string? RateLimitKey { get; }
    public Dictionary<string, object> Metadata { get; }

    public JobExecutionContext(
        string jobType, 
        string targetResource, 
        string? rateLimitKey = null,
        Dictionary<string, object>? metadata = null)
    {
        JobType = jobType ?? throw new ArgumentNullException(nameof(jobType));
        TargetResource = targetResource ?? throw new ArgumentNullException(nameof(targetResource));
        RateLimitKey = rateLimitKey;
        Metadata = metadata ?? new Dictionary<string, object>();
    }

    /// <summary>
    /// Creates a rate limit key based on the job type and target resource if none is explicitly provided
    /// </summary>
    public string GetEffectiveRateLimitKey()
    {
        return RateLimitKey ?? $"{JobType}:{TargetResource}";
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return JobType;
        yield return TargetResource;
        yield return RateLimitKey ?? string.Empty;

        // For metadata comparison, we need to consider the key-value pairs
        foreach (var kvp in Metadata.OrderBy(x => x.Key))
        {
            yield return kvp.Key;
            yield return kvp.Value;
        }
    }
}
