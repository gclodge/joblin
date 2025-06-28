using Joblin.Infrastructure;
using Joblin.Middleware;
using Joblin.Persistence;
using Joblin.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Joblin;

public static class DependencyInjection
{
    public static IServiceCollection AddJoblin(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JoblinOptions>(configuration.GetSection(JoblinOptions.SectionName));

        // Core services
        services.AddScoped<IJoblinManager, JoblinManager>();
        services.AddScoped<IJoblinWebhookService, JoblinWebhookService>();

        // Queue implementation
        services.AddScoped<IJoblinQueue, AzureStorageJobQueue>();
        
        return services;
    }

    public static IServiceCollection AddJoblinWithPostgres(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JoblinOptions>(configuration.GetSection(JoblinOptions.SectionName));

        //< Get the configured options to check EnableController
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<JoblinOptions>>().Value;

        var connectionString = configuration.GetConnectionString("JoblinDb")
            ?? throw new InvalidOperationException("JoblinDb connection string is not configured.");

        //< Add Postgres DbContext
        var dbsBuilder = new Npgsql.NpgsqlDataSourceBuilder(connectionString);
        var dbds = dbsBuilder.EnableDynamicJson().Build();
        services.AddDbContext<JoblinDbContext>(opts => opts.UseNpgsql(dbds));
        
        services.AddScoped<IJoblinDbContext>(sp => sp.GetRequiredService<JoblinDbContext>());

        //< Core services
        services.AddScoped<IJoblinManager, JoblinManager>();
        services.AddScoped<IJoblinWebhookService, JoblinWebhookService>();
        services.AddScoped<IJoblinQueue, AzureStorageJobQueue>();
        services.AddScoped<IJoblinStatusTracker, JoblinStatusTracker>();

        //< Auto-register the webhook controller if enabled
        if (options.WebhookEndpoints.EnableController)
        {
            services.AddControllers()
                .ConfigureApplicationPartManager(manager =>
                {
                    manager.FeatureProviders.Add(new JoblinControllerFeatureProvider());
                });

            // Add authentication middleware if required
            if (options.WebhookEndpoints.RequireAuthentication)
            {
                services.AddTransient<JoblinAuthenticationMiddleware>();
            }
        }

        return services;
    }

    public static IServiceCollection AddJoblinWithCustomDb<TContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<DbContextOptionsBuilder> dbOptions,
        Action<JoblinOptions>? configure = null)
        where TContext : DbContext, IJoblinDbContext
    {
        // Configure from appsettings.json/environment variables first
        services.Configure<JoblinOptions>(configuration.GetSection(JoblinOptions.SectionName));
        
        // Apply additional configuration if provided
        if (configure != null)
        {
            services.Configure<JoblinOptions>(configure);
        }

        // Get the configured options to check EnableController
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<JoblinOptions>>().Value;

        //< Register the custom DbContext
        services.AddDbContext<TContext>(dbOptions);
        services.AddScoped<IJoblinDbContext>(sp => sp.GetRequiredService<TContext>());

        //< Core Joblin services
        services.AddScoped<IJoblinManager, JoblinManager>();
        services.AddScoped<IJoblinWebhookService, JoblinWebhookService>();
        services.AddScoped<IJoblinQueue, AzureStorageJobQueue>();
        services.AddScoped<IJoblinStatusTracker, JoblinStatusTracker>();

        //< Auto-register the webhook controller if enabled
        if (options.WebhookEndpoints.EnableController)
        {
            services.AddControllers()
                .ConfigureApplicationPartManager(manager =>
                {
                    manager.FeatureProviders.Add(new JoblinControllerFeatureProvider());
                });

            // Add authentication middleware if required
            if (options.WebhookEndpoints.RequireAuthentication)
            {
                services.AddTransient<JoblinAuthenticationMiddleware>();
            }
        }

        return services;
    }

    public static void ApplyJoblinMigrations(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<JoblinDbContext>();
        context.Database.Migrate();
    }
}