using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using GitHubIntegration.Application.Exceptions;
using GitHubIntegration.Application.Integrations.GitHub;
using Microsoft.Extensions.Options;

namespace GitHubIntegration.Infrastructure.GitHub;

public sealed class GitHubClient(HttpClient httpClient, IOptions<GitHubOptions> options) : IGitHubClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly GitHubOptions _options = options.Value;

    public async Task<ExternalRepository> GetRepositoryAsync(string owner, string name, CancellationToken cancellationToken)
    {
        var response = await GetAsync<GitHubRepositoryResponse>(RepoPath(owner, name), cancellationToken);
        return new ExternalRepository(
            response.Id,
            response.Owner.Login,
            response.Name,
            response.FullName,
            response.Description,
            response.DefaultBranch,
            response.Private,
            response.StargazersCount,
            response.ForksCount,
            response.OpenIssuesCount,
            response.CreatedAt,
            response.UpdatedAt);
    }

    public async Task<IReadOnlyList<ExternalIssue>> GetIssuesAsync(string owner, string name, CancellationToken cancellationToken)
    {
        var items = await GetPagedAsync<GitHubIssueResponse>(
            page => $"{RepoPath(owner, name)}/issues?state=all&per_page={PageSize}&page={page}",
            cancellationToken);

        return items.Select(x => new ExternalIssue(
            x.Id,
            x.Number,
            x.Title,
            x.Body,
            x.State,
            x.User?.Login,
            x.Comments,
            x.CreatedAt,
            x.UpdatedAt,
            x.ClosedAt,
            x.PullRequest.HasValue)).ToList();
    }

    public async Task<IReadOnlyList<ExternalPullRequest>> GetPullRequestsAsync(string owner, string name, CancellationToken cancellationToken)
    {
        var items = await GetPagedAsync<GitHubPullRequestResponse>(
            page => $"{RepoPath(owner, name)}/pulls?state=all&per_page={PageSize}&page={page}",
            cancellationToken);

        return items.Select(x => new ExternalPullRequest(
            x.Id,
            x.Number,
            x.Title,
            x.Body,
            x.State,
            x.User?.Login,
            x.Head.Ref,
            x.Base.Ref,
            x.Draft,
            x.MergedAt.HasValue,
            x.CreatedAt,
            x.UpdatedAt,
            x.MergedAt,
            x.ClosedAt)).ToList();
    }

    public async Task<IReadOnlyList<ExternalCommit>> GetCommitsAsync(
        string owner,
        string name,
        string branch,
        CancellationToken cancellationToken)
    {
        var escapedBranch = Uri.EscapeDataString(branch);
        var items = await GetPagedAsync<GitHubCommitResponse>(
            page => $"{RepoPath(owner, name)}/commits?sha={escapedBranch}&per_page={PageSize}&page={page}",
            cancellationToken);

        return items.Select(x => new ExternalCommit(
            x.Sha,
            branch,
            x.Commit.Message,
            x.Commit.Author?.Name,
            x.Commit.Author?.Email,
            x.Author?.Login,
            x.Commit.Author?.Date ?? x.Commit.Committer?.Date ?? DateTimeOffset.MinValue,
            x.Commit.Verification?.Verified ?? false)).ToList();
    }

    private int PageSize => Math.Clamp(_options.PageSize, 1, 100);
    private int MaxPages => Math.Clamp(_options.MaxPages, 1, 20);

    private async Task<IReadOnlyList<T>> GetPagedAsync<T>(Func<int, string> pathFactory, CancellationToken cancellationToken)
    {
        var result = new List<T>();
        for (var page = 1; page <= MaxPages; page++)
        {
            var batch = await GetAsync<List<T>>(pathFactory(page), cancellationToken);
            result.AddRange(batch);
            if (batch.Count < PageSize)
            {
                break;
            }
        }

        return result;
    }

    private async Task<T> GetAsync<T>(string path, CancellationToken cancellationToken)
    {
        using var response = await httpClient.GetAsync(path, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            await ThrowExternalServiceExceptionAsync(response, cancellationToken);
        }

        var payload = await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
        return payload ?? throw new ExternalServiceException("GitHub returned an empty response payload.", (int)response.StatusCode);
    }

    private static async Task ThrowExternalServiceExceptionAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        string? apiMessage = null;
        try
        {
            using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync(cancellationToken));
            if (document.RootElement.TryGetProperty("message", out var messageElement))
            {
                apiMessage = messageElement.GetString();
            }
        }
        catch (JsonException)
        {
            // Preserve the HTTP status even if GitHub returns a non-JSON error body.
        }

        var resetAt = TryGetRateLimitReset(response);
        var remaining = TryGetHeader(response, "X-RateLimit-Remaining");
        var isRateLimited = response.StatusCode == HttpStatusCode.TooManyRequests || remaining == "0";

        var message = isRateLimited
            ? $"GitHub API rate limit exceeded. Reset at {resetAt?.ToString("O") ?? "unknown"}."
            : $"GitHub API request failed with {(int)response.StatusCode} ({response.StatusCode}): {apiMessage ?? "unknown error"}.";

        throw new ExternalServiceException(message, (int)response.StatusCode, isRateLimited ? resetAt : null);
    }

    private static DateTimeOffset? TryGetRateLimitReset(HttpResponseMessage response)
    {
        var value = TryGetHeader(response, "X-RateLimit-Reset");
        if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var unixSeconds))
        {
            return DateTimeOffset.FromUnixTimeSeconds(unixSeconds);
        }
        return null;
    }

    private static string? TryGetHeader(HttpResponseMessage response, string name) =>
        response.Headers.TryGetValues(name, out var values) ? values.FirstOrDefault() : null;

    private static string RepoPath(string owner, string name) =>
        $"repos/{Uri.EscapeDataString(owner.Trim())}/{Uri.EscapeDataString(name.Trim())}";
}
