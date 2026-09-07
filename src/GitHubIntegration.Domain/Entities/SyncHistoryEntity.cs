using GitHubIntegration.Domain.Enums;

namespace GitHubIntegration.Domain.Entities;

public sealed class SyncHistoryEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RepositoryId { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? FinishedAt { get; set; }
    public SyncStatus Status { get; set; }
    public int SyncedIssues { get; set; }
    public int SyncedPullRequests { get; set; }
    public int SyncedCommits { get; set; }
    public string? ErrorMessage { get; set; }

    public RepositoryEntity Repository { get; set; } = null!;
}
