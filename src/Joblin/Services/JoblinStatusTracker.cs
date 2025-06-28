using Microsoft.EntityFrameworkCore;

using Joblin.Persistence;
using Joblin.Persistence.Entities;

namespace Joblin.Services;

public class JoblinStatusTracker(
    IJoblinDbContext context)
    : IJoblinStatusTracker
{
    private readonly IJoblinDbContext _context = context;

    public async Task UpdateStatusAsync(string jobId, JobStatus status, object? result = null, string? errorMessage = null, CancellationToken cancellationToken = default)
    {
        var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == jobId, cancellationToken);
        
        if (job == null)
        {
            job = new JobEntity 
            { 
                Id = jobId, 
                Status = status, 
                SubmittedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Jobs.Add(job);
        }
        else
        {
            job.Status = status;
            job.UpdatedAt = DateTime.UtcNow;
            
            if (status == JobStatus.Running && job.StartedAt == null)
                job.StartedAt = DateTime.UtcNow;
            
            if (status is JobStatus.Completed or JobStatus.Failed or JobStatus.Cancelled)
                job.CompletedAt = DateTime.UtcNow;
        }
        
        if (result != null) job.Result = result;
        if (errorMessage != null) job.ErrorMessage = errorMessage;
        
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<JobStatus> GetStatusAsync(string jobId, CancellationToken cancellationToken = default)
    {
        var job = await _context.Jobs
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == jobId, cancellationToken);
            
        return job?.Status ?? throw new InvalidOperationException($"Job {jobId} not found");
    }

    public async Task<JobDetail?> GetJobAsync(string jobId, CancellationToken cancellationToken = default)
    {
        var job = await _context.Jobs
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == jobId, cancellationToken);
            
        return job == null ? null : MapToJobDetail(job);
    }

    public async Task<IEnumerable<JobSummary>> GetJobsAsync(JobFilter? filter = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Jobs.AsNoTracking().AsQueryable();
        
        if (filter?.Status.HasValue == true)
            query = query.Where(j => j.Status == filter.Status.Value);
            
        if (filter?.SubmittedAfter.HasValue == true)
            query = query.Where(j => j.SubmittedAt >= filter.SubmittedAfter.Value);
            
        if (filter?.SubmittedBefore.HasValue == true)
            query = query.Where(j => j.SubmittedAt <= filter.SubmittedBefore.Value);
            
        query = query.OrderByDescending(j => j.SubmittedAt);
        
        if (filter?.Limit.HasValue == true)
            query = query.Take(filter.Limit.Value);
            
        var jobs = await query.ToListAsync(cancellationToken);
        
        return jobs.Select(j => new JobSummary
        {
            Id = j.Id,
            SubmittedAt = j.SubmittedAt,
            Status = j.Status,
            Priority = j.Priority
        });
    }

    public async Task RecordHeartbeatAsync(string jobId, int? progress = null, string? message = null, CancellationToken cancellationToken = default)
    {
        var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == jobId, cancellationToken);
        
        if (job != null)
        {
            job.LastHeartbeat = DateTime.UtcNow;
            job.UpdatedAt = DateTime.UtcNow;
            
            if (progress.HasValue)
                job.Progress = progress.Value;
                
            if (!string.IsNullOrEmpty(message))
                job.HeartbeatMessage = message;
            
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
    
    private static JobDetail MapToJobDetail(JobEntity entity) => new()
    {
        Id = entity.Id,
        Data = entity.Data,
        WebhookUrl = entity.WebhookUrl,
        SubmittedAt = entity.SubmittedAt,
        StartedAt = entity.StartedAt,
        CompletedAt = entity.CompletedAt,
        Status = entity.Status,
        Result = entity.Result,
        ErrorMessage = entity.ErrorMessage,
        Options = new JobOptions
        {
            MaxRetries = entity.MaxRetries,
            Timeout = entity.Timeout,
            Priority = entity.Priority,
            Metadata = entity.Metadata
        }
    };
}