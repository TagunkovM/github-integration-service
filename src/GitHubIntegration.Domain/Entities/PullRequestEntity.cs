using GitHubIntegration.Domain.Enums;

namespace GitHubIntegration.Domain.Entities;

public sealed class PullRequestEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public long GitHubId { get; set; }
    public Guid RepositoryId { get; set; }
    public int Number { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Body { get; set; }
    public PullRequestState State { get; set; }
    public string? AuthorLogin { get; set; }
    public string SourceBranch { get; set; } = string.Empty;
    public string TargetBranch { get; set; } = string.Empty;
    public bool IsDraft { get; set; }
    public bool IsMerged { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? MergedAt { get; set; }
    public DateTimeOffset? ClosedAt { get; set; }

    public RepositoryEntity Repository { get; set; } = null!;
}
