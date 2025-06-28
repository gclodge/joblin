using System.Threading;
using System.Threading.Tasks;
using Joblin.Models;

namespace Joblin.Abstractions;

public interface IJoblinQueue
{
    Task EnqueueAsync(JobSubmission job, CancellationToken cancellationToken = default);
    Task<JobSubmission?> DequeueAsync(CancellationToken cancellationToken = default);
}
