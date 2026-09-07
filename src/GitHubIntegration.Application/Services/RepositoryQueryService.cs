using GitHubIntegration.Application.Abstractions;
using GitHubIntegration.Application.Common;
using GitHubIntegration.Application.DTOs;
using GitHubIntegration.Application.Exceptions;
using GitHubIntegration.Application.Mappings;
using GitHubIntegration.Domain.Enums;

namespace GitHubIntegration.Application.Services;

public sealed class RepositoryQueryService(IIntegrationStore store, ICacheService cache)
{
    private static readonly TimeSpan QueryCacheTtl = TimeSpan.FromMinutes(5);

    public async Task<PagedResult<IssueDto>> GetIssuesAsync(
        string owner,
        string name,
        int page,
        int pageSize,
        string? state,
        string? author,
        CancellationToken cancellationToken)
    {
        var repository = await GetRepositoryAsync(owner, name, cancellationToken);
        var request = Pagination.Normalize(page, pageSize);
        var parsedState = ParseIssueState(state);
        var normalizedAuthor = NormalizeOptional(author);
        var key = CacheKeys.Issues(owner, name, GetVersion(repository.LastSyncedAt), request.Page, request.PageSize, state, normalizedAuthor);

        var cached = await cache.GetAsync<PagedResult<IssueDto>>(key, cancellationToken);
        if (cached is not null)
        {
            return cached;
        }

        var result = await store.GetIssuesAsync(repository.Id, request.Page, request.PageSize, parsedState, normalizedAuthor, cancellationToken);
        var dto = new PagedResult<IssueDto>(result.Items.Select(x => x.ToDto()).ToList(), result.Page, result.PageSize, result.TotalCount);
        await cache.SetAsync(key, dto, QueryCacheTtl, cancellationToken);
        return dto;
    }

    public async Task<PagedResult<PullRequestDto>> GetPullRequestsAsync(
        string owner,
        string name,
        int page,
        int pageSize,
        string? state,
        string? author,
        CancellationToken cancellationToken)
    {
        var repository = await GetRepositoryAsync(owner, name, cancellationToken);
        var request = Pagination.Normalize(page, pageSize);
        var parsedState = ParsePullRequestState(state);
        var normalizedAuthor = NormalizeOptional(author);
        var key = CacheKeys.PullRequests(owner, name, GetVersion(repository.LastSyncedAt), request.Page, request.PageSize, state, normalizedAuthor);

        var cached = await cache.GetAsync<PagedResult<PullRequestDto>>(key, cancellationToken);
        if (cached is not null)
        {
            return cached;
        }

        var result = await store.GetPullRequestsAsync(repository.Id, request.Page, request.PageSize, parsedState, normalizedAuthor, cancellationToken);
        var dto = new PagedResult<PullRequestDto>(result.Items.Select(x => x.ToDto()).ToList(), result.Page, result.PageSize, result.TotalCount);
        await cache.SetAsync(key, dto, QueryCacheTtl, cancellationToken);
        return dto;
    }

    public async Task<PagedResult<CommitDto>> GetCommitsAsync(
        string owner,
        string name,
        int page,
        int pageSize,
        string? branch,
        CancellationToken cancellationToken)
    {
        var repository = await GetRepositoryAsync(owner, name, cancellationToken);
        var request = Pagination.Normalize(page, pageSize);
        var normalizedBranch = NormalizeOptional(branch);
        var key = CacheKeys.Commits(owner, name, GetVersion(repository.LastSyncedAt), request.Page, request.PageSize, normalizedBranch);

        var cached = await cache.GetAsync<PagedResult<CommitDto>>(key, cancellationToken);
        if (cached is not null)
        {
            return cached;
        }

        var result = await store.GetCommitsAsync(repository.Id, request.Page, request.PageSize, normalizedBranch, cancellationToken);
        var dto = new PagedResult<CommitDto>(result.Items.Select(x => x.ToDto()).ToList(), result.Page, result.PageSize, result.TotalCount);
        await cache.SetAsync(key, dto, QueryCacheTtl, cancellationToken);
        return dto;
    }

    private async Task<GitHubIntegration.Domain.Entities.RepositoryEntity> GetRepositoryAsync(string owner, string name, CancellationToken cancellationToken) =>
        await store.GetRepositoryAsync(owner, name, false, cancellationToken)
        ?? throw new NotFoundException($"Repository '{owner}/{name}' is not tracked.");

    private static long GetVersion(DateTimeOffset? value) => value?.ToUnixTimeMilliseconds() ?? 0L;

    private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static IssueState? ParseIssueState(string? state) => state?.Trim().ToLowerInvariant() switch
    {
        null or "" => null,
        "open" => IssueState.Open,
        "closed" => IssueState.Closed,
        _ => throw new RequestValidationException("Issue state must be 'open' or 'closed'.")
    };

    private static PullRequestState? ParsePullRequestState(string? state) => state?.Trim().ToLowerInvariant() switch
    {
        null or "" => null,
        "open" => PullRequestState.Open,
        "closed" => PullRequestState.Closed,
        _ => throw new RequestValidationException("Pull request state must be 'open' or 'closed'.")
    };
}
