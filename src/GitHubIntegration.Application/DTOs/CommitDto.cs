namespace GitHubIntegration.Application.DTOs;

public sealed record CommitDto(
    string Sha,
    string Branch,
    string Message,
    string? AuthorName,
    string? AuthorEmail,
    string? AuthorLogin,
    DateTimeOffset CommittedAt,
    bool IsVerified);
