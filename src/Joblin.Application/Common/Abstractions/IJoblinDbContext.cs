using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Joblin.Application.Common.Abstractions;

/// <summary>
/// An interface representing the database context for the Joblin application.
/// </summary>
public interface IJoblinDbContext
{
    /// <summary>
    /// Gets the set of <see cref="Job"/> entities in the database.
    /// </summary>
    public DbSet<Job> Jobs { get; }

    /// <summary>
    /// Gets the set of <see cref="RateLimitConfiguration"/> entities in the database.
    /// </summary>
    public DbSet<RateLimitConfiguration> RateLimitConfigurations { get; }

    /// <summary>
    /// Gets the set of <see cref="RateLimitState"/> entities in the database.
    /// </summary>
    public DbSet<RateLimitState> RateLimitStates { get; }    

    /// <summary>
    /// Provides access to the underlying <see cref="DatabaseFacade"/> for transactions and other database operations.
    /// </summary>
    public DatabaseFacade Database { get; }

    /// <summary>
    /// Persist the currently tracked/stages changes to the backing database.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
