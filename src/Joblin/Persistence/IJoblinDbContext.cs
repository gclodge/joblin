using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Joblin.Persistence.Entities;

namespace Joblin.Persistence;

public interface IJoblinDbContext
{
    DbSet<JobEntity> Jobs { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
