using GitHubIntegration.Application.Integrations.GitHub;
using GitHubIntegration.Domain.Entities;
using GitHubIntegration.Domain.Enums;

namespace GitHubIntegration.Application.Mappings;

public static class GitHubMappings
{
    public static RepositoryEntity ToRepositoryEntity(ExternalRepository source) => new()
    {
        GitHubId = source.GitHubId,
        Owner = source.Owner,
        Name = source.Name,
        FullName = source.FullName,
        Description = source.Description,
        DefaultBranch = source.DefaultBranch,
        IsPrivate = source.IsPrivate,
        StarsCount = source.StarsCount,
        ForksCount = source.ForksCount,
        OpenIssuesCount = source.OpenIssuesCount,
        GitHubCreatedAt = source.CreatedAt,
        GitHubUpdatedAt = source.UpdatedAt
    };

    public static void Apply(RepositoryEntity target, ExternalRepository source)
    {
        target.GitHubId = source.GitHubId;
        target.Owner = source.Owner;
        target.Name = source.Name;
        target.FullName = source.FullName;
        target.Description = source.Description;
        target.DefaultBranch = source.DefaultBranch;
        target.IsPrivate = source.IsPrivate;
        target.StarsCount = source.StarsCount;
        target.ForksCount = source.ForksCount;
        target.OpenIssuesCount = source.OpenIssuesCount;
        target.GitHubCreatedAt = source.CreatedAt;
        target.GitHubUpdatedAt = source.UpdatedAt;
    }

    public static IReadOnlyList<IssueEntity> ToIssueEntities(Guid repositoryId, IEnumerable<ExternalIssue> source) =>
        source
            .Where(x => !x.IsPullRequest)
            .Select(x => new IssueEntity
            {
                RepositoryId = repositoryId,
                GitHubId = x.GitHubId,
                Number = x.Number,
                Title = x.Title,
                Body = x.Body,
                State = ParseIssueState(x.State),
                AuthorLogin = x.AuthorLogin,
                CommentsCount = x.CommentsCount,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                ClosedAt = x.ClosedAt
            })
            .ToList();

    public static IReadOnlyList<PullRequestEntity> ToPullRequestEntities(Guid repositoryId, IEnumerable<ExternalPullRequest> source) =>
        source.Select(x => new PullRequestEntity
        {
            RepositoryId = repositoryId,
            GitHubId = x.GitHubId,
            Number = x.Number,
            Title = x.Title,
            Body = x.Body,
            State = ParsePullRequestState(x.State),
            AuthorLogin = x.AuthorLogin,
            SourceBranch = x.SourceBranch,
            TargetBranch = x.TargetBranch,
            IsDraft = x.IsDraft,
            IsMerged = x.IsMerged,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt,
            MergedAt = x.MergedAt,
            ClosedAt = x.ClosedAt
        }).ToList();

    public static IReadOnlyList<CommitEntity> ToCommitEntities(Guid repositoryId, IEnumerable<ExternalCommit> source) =>
        source.Select(x => new CommitEntity
        {
            RepositoryId = repositoryId,
            Sha = x.Sha,
            Branch = x.Branch,
            Message = x.Message,
            AuthorName = x.AuthorName,
            AuthorEmail = x.AuthorEmail,
            AuthorLogin = x.AuthorLogin,
            CommittedAt = x.CommittedAt,
            IsVerified = x.IsVerified
        }).ToList();

    private static IssueState ParseIssueState(string value) =>
        value.Equals("closed", StringComparison.OrdinalIgnoreCase) ? IssueState.Closed : IssueState.Open;

    private static PullRequestState ParsePullRequestState(string value) =>
        value.Equals("closed", StringComparison.OrdinalIgnoreCase) ? PullRequestState.Closed : PullRequestState.Open;
}
