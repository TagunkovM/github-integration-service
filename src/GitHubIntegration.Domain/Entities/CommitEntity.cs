namespace GitHubIntegration.Domain.Entities;

public sealed class CommitEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RepositoryId { get; set; }
    public string Sha { get; set; } = string.Empty;
    public string Branch { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? AuthorName { get; set; }
    public string? AuthorEmail { get; set; }
    public string? AuthorLogin { get; set; }
    public DateTimeOffset CommittedAt { get; set; }
    public bool IsVerified { get; set; }

    public RepositoryEntity Repository { get; set; } = null!;
}
