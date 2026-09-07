namespace GitHubIntegration.Domain.Entities;

public sealed class RepositoryEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public long GitHubId { get; set; }
    public string Owner { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DefaultBranch { get; set; } = string.Empty;
    public bool IsPrivate { get; set; }
    public int StarsCount { get; set; }
    public int ForksCount { get; set; }
    public int OpenIssuesCount { get; set; }
    public DateTimeOffset GitHubCreatedAt { get; set; }
    public DateTimeOffset GitHubUpdatedAt { get; set; }
    public DateTimeOffset? LastSyncedAt { get; set; }

    public ICollection<IssueEntity> Issues { get; set; } = new List<IssueEntity>();
    public ICollection<PullRequestEntity> PullRequests { get; set; } = new List<PullRequestEntity>();
    public ICollection<CommitEntity> Commits { get; set; } = new List<CommitEntity>();
    public ICollection<SyncHistoryEntity> SyncHistory { get; set; } = new List<SyncHistoryEntity>();
}
