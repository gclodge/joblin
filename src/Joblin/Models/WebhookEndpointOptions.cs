namespace Joblin.Models;

public class WebhookEndpointOptions
{
    public string BaseRoute { get; set; } = "api/joblin/webhooks";
    public string StatusUpdateRoute { get; set; } = "status";
    public string HeartbeatRoute { get; set; } = "heartbeat";
    public bool EnableController { get; set; } = true;
    public bool RequireAuthentication { get; set; } = false;
    public string? AuthenticationScheme { get; set; }
    public string? ApiKey { get; set; }
}
