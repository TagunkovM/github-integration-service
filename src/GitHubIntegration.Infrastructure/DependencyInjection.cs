using System.Net.Http.Headers;
using GitHubIntegration.Application.Abstractions;
using GitHubIntegration.Application.Integrations.GitHub;
using GitHubIntegration.Infrastructure.Background;
using GitHubIntegration.Infrastructure.Caching;
using GitHubIntegration.Infrastructure.GitHub;
using GitHubIntegration.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace GitHubIntegration.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseConnection = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("ConnectionStrings:Database is required.");
        var redisConnection = configuration.GetConnectionString("Redis")
            ?? throw new InvalidOperationException("ConnectionStrings:Redis is required.");

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(databaseConnection, npgsql => npgsql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(5), null)));
        services.AddScoped<IIntegrationStore, EfIntegrationStore>();

        services.AddStackExchangeRedisCache(options => options.Configuration = redisConnection);
        services.AddSingleton<ICacheService, RedisCacheService>();
        services.AddSingleton<ISyncJobQueue, SyncJobQueue>();

        services.AddOptions<GitHubOptions>()
            .Bind(configuration.GetSection(GitHubOptions.SectionName))
            .Validate(x => Uri.TryCreate(x.BaseUrl, UriKind.Absolute, out _), "GitHub:BaseUrl must be an absolute URL.")
            .Validate(x => !string.IsNullOrWhiteSpace(x.Token), "GitHub:Token is required for PAT authentication.")
            .Validate(x => x.PageSize is >= 1 and <= 100, "GitHub:PageSize must be between 1 and 100.")
            .Validate(x => x.MaxPages is >= 1 and <= 20, "GitHub:MaxPages must be between 1 and 20.")
            .ValidateOnStart();

        services.AddHttpClient<IGitHubClient, GitHubClient>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<GitHubOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl, UriKind.Absolute);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
            client.DefaultRequestHeaders.UserAgent.ParseAdd("GitHubIntegrationService/1.0");
            client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", options.ApiVersion);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.Token);
        }).AddStandardResilienceHandler();

        return services;
    }
}
