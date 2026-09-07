namespace GitHubIntegration.Application.DTOs;

public sealed record SyncStatusDto(
    Guid Id,
    string Repository,
    string Status,
    DateTimeOffset StartedAt,
    DateTimeOffset? FinishedAt,
    int SyncedIssues,
    int SyncedPullRequests,
    int SyncedCommits,
    string? ErrorMessage);
