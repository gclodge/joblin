using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Joblin.Middleware;

public class JoblinAuthenticationMiddleware(
    RequestDelegate next,
    IOptions<JoblinOptions> options)
{
    private readonly RequestDelegate _next = next;
    private readonly JoblinOptions _options = options.Value;

    public async Task InvokeAsync(HttpContext context)
    {
        //< Only check for authentication if the request path matches the webhook endpoints
        if (context.Request.Path.StartsWithSegments(_options.WebhookEndpoints.BaseRoute))
        {
            if (_options.WebhookEndpoints.RequireAuthentication)
            {
                //< Check for API key in header
                //< TODO - Make this key a constant or configurable
                var apiKey = context.Request.Headers["X-Joblin-ApiKey"].FirstOrDefault();
                var configuredApiKey = _options.WebhookEndpoints.ApiKey;

                if (string.IsNullOrEmpty(apiKey) || apiKey != configuredApiKey)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized: Invalid or missing API key");
                    return;
                }
            }
        }

        await _next(context);
    }
}