using System;
using System.Collections.Generic;
using Joblin.Models;

namespace Joblin.Persistence.Entities;

public class JobEntity
{
    public string Id { get; set; } = string.Empty;
    public object? Data { get; set; }
    public string WebhookUrl { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public JobStatus Status { get; set; }
    public object? Result { get; set; }
    public string? ErrorMessage { get; set; }
    public int MaxRetries { get; set; }
    public int RetryCount { get; set; }
    public TimeSpan? Timeout { get; set; }
    public int Priority { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
    public DateTime UpdatedAt { get; set; }
    public int? Progress { get; set; }
    public string? HeartbeatMessage { get; set; }
    public DateTime? LastHeartbeat { get; set; }
}
