using GitHubIntegration.Domain.Enums;

namespace GitHubIntegration.Domain.Entities;

public sealed class IssueEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public long GitHubId { get; set; }
    public Guid RepositoryId { get; set; }
    public int Number { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Body { get; set; }
    public IssueState State { get; set; }
    public string? AuthorLogin { get; set; }
    public int CommentsCount { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? ClosedAt { get; set; }

    public RepositoryEntity Repository { get; set; } = null!;
}
