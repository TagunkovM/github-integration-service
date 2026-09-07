namespace GitHubIntegration.Application.DTOs;

public sealed record IssueDto(
    int Number,
    string Title,
    string? Body,
    string State,
    string? AuthorLogin,
    int CommentsCount,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? ClosedAt);
