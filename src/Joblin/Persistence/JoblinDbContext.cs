using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Joblin.Persistence.Entities;

namespace Joblin.Persistence;

public class JoblinDbContext : DbContext, IJoblinDbContext
{
    public JoblinDbContext(DbContextOptions<JoblinDbContext> options) : base(options) { }
    
    public DbSet<JobEntity> Jobs { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<JobEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.WebhookUrl).HasMaxLength(2000);
            entity.Property(e => e.Status).HasConversion<string>();
            
            // Store job data as JSON
            entity.Property(e => e.Data)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => v == null ? null : JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => v == null ? null : JsonSerializer.Deserialize<object>(v, (JsonSerializerOptions?)null));
                    
            entity.Property(e => e.Result)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => v == null ? null : JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => v == null ? null : JsonSerializer.Deserialize<object>(v, (JsonSerializerOptions?)null));
                    
            entity.Property(e => e.Metadata)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions?)null) ?? new());
                    
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.SubmittedAt);
            entity.HasIndex(e => new { e.Status, e.SubmittedAt });
        });
    }
}
