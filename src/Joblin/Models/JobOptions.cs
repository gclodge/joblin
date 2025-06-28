namespace Joblin.Models;

public class JobOptions
{
    public int MaxRetries { get; set; } = 3;
    public TimeSpan? Timeout { get; set; }
    public int Priority { get; set; } = 0;
    public Dictionary<string, string> Metadata { get; set; } = new();
}
