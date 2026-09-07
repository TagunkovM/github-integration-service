using System.Text.Json;
using System.Text.Json.Serialization;

namespace GitHubIntegration.Infrastructure.GitHub;

internal sealed class GitHubRepositoryResponse
{
    [JsonPropertyName("id")] public long Id { get; init; }
    [JsonPropertyName("name")] public string Name { get; init; } = string.Empty;
    [JsonPropertyName("full_name")] public string FullName { get; init; } = string.Empty;
    [JsonPropertyName("description")] public string? Description { get; init; }
    [JsonPropertyName("private")] public bool Private { get; init; }
    [JsonPropertyName("default_branch")] public string DefaultBranch { get; init; } = string.Empty;
    [JsonPropertyName("stargazers_count")] public int StargazersCount { get; init; }
    [JsonPropertyName("forks_count")] public int ForksCount { get; init; }
    [JsonPropertyName("open_issues_count")] public int OpenIssuesCount { get; init; }
    [JsonPropertyName("created_at")] public DateTimeOffset CreatedAt { get; init; }
    [JsonPropertyName("updated_at")] public DateTimeOffset UpdatedAt { get; init; }
    [JsonPropertyName("owner")] public GitHubUserResponse Owner { get; init; } = new();
}

internal sealed class GitHubIssueResponse
{
    [JsonPropertyName("id")] public long Id { get; init; }
    [JsonPropertyName("number")] public int Number { get; init; }
    [JsonPropertyName("title")] public string Title { get; init; } = string.Empty;
    [JsonPropertyName("body")] public string? Body { get; init; }
    [JsonPropertyName("state")] public string State { get; init; } = "open";
    [JsonPropertyName("user")] public GitHubUserResponse? User { get; init; }
    [JsonPropertyName("comments")] public int Comments { get; init; }
    [JsonPropertyName("created_at")] public DateTimeOffset CreatedAt { get; init; }
    [JsonPropertyName("updated_at")] public DateTimeOffset UpdatedAt { get; init; }
    [JsonPropertyName("closed_at")] public DateTimeOffset? ClosedAt { get; init; }
    [JsonPropertyName("pull_request")] public JsonElement? PullRequest { get; init; }
}

internal sealed class GitHubPullRequestResponse
{
    [JsonPropertyName("id")] public long Id { get; init; }
    [JsonPropertyName("number")] public int Number { get; init; }
    [JsonPropertyName("title")] public string Title { get; init; } = string.Empty;
    [JsonPropertyName("body")] public string? Body { get; init; }
    [JsonPropertyName("state")] public string State { get; init; } = "open";
    [JsonPropertyName("user")] public GitHubUserResponse? User { get; init; }
    [JsonPropertyName("head")] public GitHubBranchResponse Head { get; init; } = new();
    [JsonPropertyName("base")] public GitHubBranchResponse Base { get; init; } = new();
    [JsonPropertyName("draft")] public bool Draft { get; init; }
    [JsonPropertyName("created_at")] public DateTimeOffset CreatedAt { get; init; }
    [JsonPropertyName("updated_at")] public DateTimeOffset UpdatedAt { get; init; }
    [JsonPropertyName("merged_at")] public DateTimeOffset? MergedAt { get; init; }
    [JsonPropertyName("closed_at")] public DateTimeOffset? ClosedAt { get; init; }
}

internal sealed class GitHubCommitResponse
{
    [JsonPropertyName("sha")] public string Sha { get; init; } = string.Empty;
    [JsonPropertyName("commit")] public GitHubCommitDetailsResponse Commit { get; init; } = new();
    [JsonPropertyName("author")] public GitHubUserResponse? Author { get; init; }
}

internal sealed class GitHubCommitDetailsResponse
{
    [JsonPropertyName("message")] public string Message { get; init; } = string.Empty;
    [JsonPropertyName("author")] public GitHubCommitAuthorResponse? Author { get; init; }
    [JsonPropertyName("committer")] public GitHubCommitAuthorResponse? Committer { get; init; }
    [JsonPropertyName("verification")] public GitHubVerificationResponse? Verification { get; init; }
}

internal sealed class GitHubCommitAuthorResponse
{
    [JsonPropertyName("name")] public string? Name { get; init; }
    [JsonPropertyName("email")] public string? Email { get; init; }
    [JsonPropertyName("date")] public DateTimeOffset? Date { get; init; }
}

internal sealed class GitHubVerificationResponse
{
    [JsonPropertyName("verified")] public bool Verified { get; init; }
}

internal sealed class GitHubBranchResponse
{
    [JsonPropertyName("ref")] public string Ref { get; init; } = string.Empty;
}

internal sealed class GitHubUserResponse
{
    [JsonPropertyName("login")] public string Login { get; init; } = string.Empty;
}
