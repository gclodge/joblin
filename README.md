# joblin

* [joblin](#joblin)
    * [Overview](#overview)

## Overview

`joblin` is a .NET-based, minimally viable, asynchronous worker management service.  Users can configure & add the business logic they need scheduled, managed, rate-limited, or otherwise executed & tracked with horizontal scalability.

Further - being a consistent & declarative framework it should allow users to quickly populate the remaining business logic using AI agent assistance.

# Configuration

Sample appsettings.json:
```json
"Joblin": {
  "DefaultWebhookTimeout": "00:00:30",
  "DefaultMaxRetries": 3,
  "QueueConnectionString": "",
  "WebhookEndpoints": {
    "BaseRoute": "/webhooks",
    "StatusUpdateRoute": "/status",
    "HeartbeatRoute": "/heartbeat",
    "EnableController": true,
    "RequireAuthentication": false,
    "AuthenticationScheme": null
  }
}
```