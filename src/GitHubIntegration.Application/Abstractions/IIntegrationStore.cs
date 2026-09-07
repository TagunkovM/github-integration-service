using GitHubIntegration.Application.Common;
using GitHubIntegration.Domain.Entities;
using GitHubIntegration.Domain.Enums;

namespace GitHubIntegration.Application.Abstractions;

public interface IIntegrationStore
{
    Task<RepositoryEntity?> GetRepositoryAsync(string owner, string name, bool tracking, CancellationToken cancellationToken);
    Task<RepositoryEntity?> GetRepositoryByIdAsync(Guid id, bool tracking, CancellationToken cancellationToken);
    Task<IReadOnlyList<RepositoryEntity>> ListRepositoriesAsync(CancellationToken cancellationToken);
    Task AddRepositoryAsync(RepositoryEntity repository, CancellationToken cancellationToken);
    Task DeleteRepositoryAsync(RepositoryEntity repository, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);

    Task<PagedResult<IssueEntity>> GetIssuesAsync(Guid repositoryId, int page, int pageSize, IssueState? state, string? author, CancellationToken cancellationToken);
    Task<PagedResult<PullRequestEntity>> GetPullRequestsAsync(Guid repositoryId, int page, int pageSize, PullRequestState? state, string? author, CancellationToken cancellationToken);
    Task<PagedResult<CommitEntity>> GetCommitsAsync(Guid repositoryId, int page, int pageSize, string? branch, CancellationToken cancellationToken);

    Task UpsertIssuesAsync(Guid repositoryId, IReadOnlyCollection<IssueEntity> issues, CancellationToken cancellationToken);
    Task UpsertPullRequestsAsync(Guid repositoryId, IReadOnlyCollection<PullRequestEntity> pullRequests, CancellationToken cancellationToken);
    Task UpsertCommitsAsync(Guid repositoryId, IReadOnlyCollection<CommitEntity> commits, CancellationToken cancellationToken);

    Task AddSyncHistoryAsync(SyncHistoryEntity syncHistory, CancellationToken cancellationToken);
    Task<SyncHistoryEntity?> GetSyncHistoryAsync(Guid syncId, bool tracking, CancellationToken cancellationToken);
    Task<SyncHistoryEntity?> GetRunningSyncAsync(Guid repositoryId, CancellationToken cancellationToken);
}
