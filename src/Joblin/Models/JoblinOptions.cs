namespace Joblin.Models;

public class JoblinOptions
{
    public const string SectionName = "Joblin";

    public string DefaultWebhookTimeout { get; set; } = "00:00:30";
    public int DefaultMaxRetries { get; set; } = 3;
    public string QueueConnectionString { get; set; } = string.Empty;
    public WebhookEndpointOptions WebhookEndpoints { get; set; } = new();
}
