namespace Joblin.Domain.Enums;

/// <summary>
/// Represents the priority level of a job
/// </summary>
public enum JobPriority
{
    /// <summary>
    /// Low priority - can be delayed significantly
    /// </summary>
    Low = 0,
    
    /// <summary>
    /// Normal priority - default for most jobs
    /// </summary>
    Normal = 10,
    
    /// <summary>
    /// High priority - should be processed soon
    /// </summary>
    High = 20,
    
    /// <summary>
    /// Critical priority - should be processed immediately if possible
    /// </summary>
    Critical = 30,
    
    /// <summary>
    /// Emergency priority - highest priority, bypasses some rate limits
    /// </summary>
    Emergency = 40
}
