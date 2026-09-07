namespace GitHubIntegration.Application.DTOs;

public sealed record PullRequestDto(
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
