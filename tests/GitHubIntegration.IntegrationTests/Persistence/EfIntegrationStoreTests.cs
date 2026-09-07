using Xunit;
using GitHubIntegration.Application.Integrations.GitHub;
using GitHubIntegration.Application.Mappings;
using GitHubIntegration.Domain.Entities;
using GitHubIntegration.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace GitHubIntegration.IntegrationTests.Persistence;

public sealed class EfIntegrationStoreTests
{
    [Fact]
    public async Task UpsertIssuesAsync_UpdatesExistingIssueInsteadOfCreatingDuplicate()
    {
        await using var postgres = new PostgreSqlBuilder("postgres:17-alpine")
            .WithDatabase("github_integration_tests")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        await postgres.StartAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(postgres.GetConnectionString())
            .Options;

        await using var db = new AppDbContext(options);
        await db.Database.EnsureCreatedAsync();

        var repository = new RepositoryEntity
        {
            GitHubId = 1,
            Owner = "octocat",
            Name = "Hello-World",
            FullName = "octocat/Hello-World",
            DefaultBranch = "main",
            GitHubCreatedAt = DateTimeOffset.UtcNow,
            GitHubUpdatedAt = DateTimeOffset.UtcNow
        };
        db.Repositories.Add(repository);
        await db.SaveChangesAsync();

        var store = new EfIntegrationStore(db);
        var first = new ExternalIssue(100, 1, "Old title", null, "open", "octocat", 0,
            DateTimeOffset.UtcNow, DateTimeOffset.UtcNow, null, false);
        var second = first with { Title = "Updated title", State = "closed" };

        await store.UpsertIssuesAsync(repository.Id, GitHubMappings.ToIssueEntities(repository.Id, [first]), CancellationToken.None);
        await store.UpsertIssuesAsync(repository.Id, GitHubMappings.ToIssueEntities(repository.Id, [second]), CancellationToken.None);

        var issues = await db.Issues.AsNoTracking().ToListAsync();
        var issue = Assert.Single(issues);
        Assert.Equal("Updated title", issue.Title);
        Assert.Equal(1, issue.Number);
    }
}
