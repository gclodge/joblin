using Azure.Storage.Queues;
using System.Text.Json;

namespace Joblin.Services;

public class AzureStorageJobQueue : IJoblinQueue
{
    private readonly QueueClient _queueClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public AzureStorageJobQueue(JoblinOptions options)
    {
        if (string.IsNullOrEmpty(options.QueueConnectionString))
            throw new ArgumentException("Queue connection string is required", nameof(options));

        _queueClient = new QueueClient(options.QueueConnectionString, "joblin-jobs");
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task EnqueueAsync(JobSubmission job, CancellationToken cancellationToken = default)
    {
        await _queueClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        
        var message = JsonSerializer.Serialize(job, _jsonOptions);
        await _queueClient.SendMessageAsync(message, cancellationToken);
    }

    public async Task<JobSubmission?> DequeueAsync(CancellationToken cancellationToken = default)
    {
        var response = await _queueClient.ReceiveMessageAsync(cancellationToken: cancellationToken);
        
        if (response.Value == null)
            return null;

        var message = response.Value;
        var job = JsonSerializer.Deserialize<JobSubmission>(message.MessageText, _jsonOptions);
        
        // Delete the message from the queue
        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, cancellationToken);
        
        return job;
    }
}
