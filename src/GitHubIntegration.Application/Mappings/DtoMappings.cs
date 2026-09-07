using GitHubIntegration.Application.DTOs;
using GitHubIntegration.Domain.Entities;

namespace GitHubIntegration.Application.Mappings;

public static class DtoMappings
{
    public static RepositoryDto ToDto(this RepositoryEntity x) => new(
        x.Id, x.GitHubId, x.Owner, x.Name, x.FullName, x.Description, x.DefaultBranch,
        x.IsPrivate, x.StarsCount, x.ForksCount, x.OpenIssuesCount,
        x.GitHubCreatedAt, x.GitHubUpdatedAt, x.LastSyncedAt);

    public static IssueDto ToDto(this IssueEntity x) => new(
        x.Number, x.Title, x.Body, x.State.ToString().ToLowerInvariant(), x.AuthorLogin,
        x.CommentsCount, x.CreatedAt, x.UpdatedAt, x.ClosedAt);

    public static PullRequestDto ToDto(this PullRequestEntity x) => new(
        x.Number, x.Title, x.Body, x.State.ToString().ToLowerInvariant(), x.AuthorLogin,
        x.SourceBranch, x.TargetBranch, x.IsDraft, x.IsMerged,
        x.CreatedAt, x.UpdatedAt, x.MergedAt, x.ClosedAt);

    public static CommitDto ToDto(this CommitEntity x) => new(
        x.Sha, x.Branch, x.Message, x.AuthorName, x.AuthorEmail, x.AuthorLogin, x.CommittedAt, x.IsVerified);

    public static SyncStatusDto ToDto(this SyncHistoryEntity x) => new(
        x.Id,
        x.Repository?.FullName ?? string.Empty,
        x.Status.ToString().ToLowerInvariant(),
        x.StartedAt,
        x.FinishedAt,
        x.SyncedIssues,
        x.SyncedPullRequests,
        x.SyncedCommits,
        x.ErrorMessage);
}
