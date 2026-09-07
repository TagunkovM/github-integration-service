using Xunit;
using GitHubIntegration.Application.Integrations.GitHub;
using GitHubIntegration.Application.Mappings;
using GitHubIntegration.Domain.Enums;

namespace GitHubIntegration.UnitTests.Mappings;

public sealed class GitHubMappingsTests
{
    [Fact]
    public void ToIssueEntities_FiltersPullRequestsReturnedByIssuesEndpoint()
    {
        var repositoryId = Guid.NewGuid();
        var source = new[]
        {
            new ExternalIssue(10, 7, "Real issue", "body", "open", "alice", 2,
                DateTimeOffset.UtcNow, DateTimeOffset.UtcNow, null, false),
            new ExternalIssue(11, 8, "Pull request in issues payload", null, "closed", "bob", 0,
                DateTimeOffset.UtcNow, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow, true)
        };

        var result = GitHubMappings.ToIssueEntities(repositoryId, source);

        var issue = Assert.Single(result);
        Assert.Equal(7, issue.Number);
        Assert.Equal(IssueState.Open, issue.State);
        Assert.Equal("alice", issue.AuthorLogin);
    }
}
