namespace GitHubIntegration.Application.Integrations.GitHub;

public interface IGitHubClient
{
    Task<ExternalRepository> GetRepositoryAsync(string owner, string name, CancellationToken cancellationToken);
    Task<IReadOnlyList<ExternalIssue>> GetIssuesAsync(string owner, string name, CancellationToken cancellationToken);
    Task<IReadOnlyList<ExternalPullRequest>> GetPullRequestsAsync(string owner, string name, CancellationToken cancellationToken);
    Task<IReadOnlyList<ExternalCommit>> GetCommitsAsync(string owner, string name, string branch, CancellationToken cancellationToken);
}
