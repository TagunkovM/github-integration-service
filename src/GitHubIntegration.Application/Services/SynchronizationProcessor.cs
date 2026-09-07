using GitHubIntegration.Application.Abstractions;
using GitHubIntegration.Application.Common;
using GitHubIntegration.Application.Exceptions;
using GitHubIntegration.Application.Integrations.GitHub;
using GitHubIntegration.Application.Mappings;
using GitHubIntegration.Domain.Enums;

namespace GitHubIntegration.Application.Services;

public sealed class SynchronizationProcessor(
    IIntegrationStore store,
    IGitHubClient gitHubClient,
    ICacheService cache)
{
    public async Task ProcessAsync(Guid syncId, CancellationToken cancellationToken)
    {
        var history = await store.GetSyncHistoryAsync(syncId, true, cancellationToken)
            ?? throw new NotFoundException($"Synchronization '{syncId}' was not found.");

        var repository = await store.GetRepositoryByIdAsync(history.RepositoryId, true, cancellationToken)
            ?? throw new NotFoundException($"Repository for synchronization '{syncId}' was not found.");

        try
        {
            var remoteRepository = await gitHubClient.GetRepositoryAsync(repository.Owner, repository.Name, cancellationToken);
            var issues = await gitHubClient.GetIssuesAsync(repository.Owner, repository.Name, cancellationToken);
            var pullRequests = await gitHubClient.GetPullRequestsAsync(repository.Owner, repository.Name, cancellationToken);
            var commits = await gitHubClient.GetCommitsAsync(repository.Owner, repository.Name, remoteRepository.DefaultBranch, cancellationToken);

            var issueEntities = GitHubMappings.ToIssueEntities(repository.Id, issues);
            var pullRequestEntities = GitHubMappings.ToPullRequestEntities(repository.Id, pullRequests);
            var commitEntities = GitHubMappings.ToCommitEntities(repository.Id, commits);

            await store.UpsertIssuesAsync(repository.Id, issueEntities, cancellationToken);
            await store.UpsertPullRequestsAsync(repository.Id, pullRequestEntities, cancellationToken);
            await store.UpsertCommitsAsync(repository.Id, commitEntities, cancellationToken);

            GitHubMappings.Apply(repository, remoteRepository);
            repository.LastSyncedAt = DateTimeOffset.UtcNow;

            history.Status = SyncStatus.Success;
            history.FinishedAt = DateTimeOffset.UtcNow;
            history.SyncedIssues = issueEntities.Count;
            history.SyncedPullRequests = pullRequestEntities.Count;
            history.SyncedCommits = commitEntities.Count;
            history.ErrorMessage = null;

            await store.SaveChangesAsync(cancellationToken);
            await cache.RemoveAsync(CacheKeys.Repository(repository.Owner, repository.Name), cancellationToken);
            await cache.RemoveAsync(CacheKeys.RepositoryList, cancellationToken);
        }
        catch (Exception ex)
        {
            history.Status = SyncStatus.Failed;
            history.FinishedAt = DateTimeOffset.UtcNow;
            history.ErrorMessage = ex.Message.Length <= 2000 ? ex.Message : ex.Message[..2000];
            await store.SaveChangesAsync(cancellationToken);
            throw;
        }
    }
}
