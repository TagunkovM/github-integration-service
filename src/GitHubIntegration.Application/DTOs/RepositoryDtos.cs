namespace GitHubIntegration.Application.DTOs;

public sealed record TrackRepositoryRequest(string Owner, string Name);

public sealed record RepositoryDto(
    Guid Id,
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
    DateTimeOffset GitHubCreatedAt,
    DateTimeOffset GitHubUpdatedAt,
    DateTimeOffset? LastSyncedAt);

public sealed record TrackRepositoryResponse(RepositoryDto Repository, Guid? InitialSyncId, bool AlreadyTracked);
