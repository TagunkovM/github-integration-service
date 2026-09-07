namespace GitHubIntegration.Application.Integrations.GitHub;

public sealed record ExternalRepository(
    long GitHubId,
    string Owner,
    string Name,
    string FullName,
    string? Description,
    string DefaultBranch,
    bool IsPrivate,
    int StarsCount,
    int ForksCount,
    int OpenIssuesCount,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record ExternalIssue(
    long GitHubId,
    int Number,
    string Title,
    string? Body,
    string State,
    string? AuthorLogin,
    int CommentsCount,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? ClosedAt,
    bool IsPullRequest);

public sealed record ExternalPullRequest(
    long GitHubId,
    int Number,
    string Title,
    string? Body,
    string State,
    string? AuthorLogin,
    string SourceBranch,
    string TargetBranch,
    bool IsDraft,
    bool IsMerged,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? MergedAt,
    DateTimeOffset? ClosedAt);

public sealed record ExternalCommit(
    string Sha,
    string Branch,
    string Message,
    string? AuthorName,
    string? AuthorEmail,
    string? AuthorLogin,
    DateTimeOffset CommittedAt,
    bool IsVerified);
