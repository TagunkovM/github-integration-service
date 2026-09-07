using GitHubIntegration.Application.Abstractions;
using GitHubIntegration.Application.Common;
using GitHubIntegration.Application.DTOs;
using GitHubIntegration.Application.Exceptions;
using GitHubIntegration.Application.Integrations.GitHub;
using GitHubIntegration.Application.Mappings;

namespace GitHubIntegration.Application.Services;

public sealed class RepositoryService(
    IIntegrationStore store,
    IGitHubClient gitHubClient,
    ICacheService cache)
{
    private static readonly TimeSpan RepositoryCacheTtl = TimeSpan.FromMinutes(5);

    public async Task<(RepositoryDto Repository, bool AlreadyTracked)> TrackAsync(
        string owner,
        string name,
        CancellationToken cancellationToken)
    {
        ValidateRepositoryCoordinates(owner, name);

        var existing = await store.GetRepositoryAsync(owner, name, false, cancellationToken);
        if (existing is not null)
        {
            return (existing.ToDto(), true);
        }

        var remote = await gitHubClient.GetRepositoryAsync(owner.Trim(), name.Trim(), cancellationToken);
        var entity = GitHubMappings.ToRepositoryEntity(remote);

        await store.AddRepositoryAsync(entity, cancellationToken);
        await cache.RemoveAsync(CacheKeys.RepositoryList, cancellationToken);
        await cache.SetAsync(CacheKeys.Repository(entity.Owner, entity.Name), entity.ToDto(), RepositoryCacheTtl, cancellationToken);

        return (entity.ToDto(), false);
    }

    public async Task<RepositoryDto> GetAsync(string owner, string name, CancellationToken cancellationToken)
    {
        ValidateRepositoryCoordinates(owner, name);
        var key = CacheKeys.Repository(owner, name);
        var cached = await cache.GetAsync<RepositoryDto>(key, cancellationToken);
        if (cached is not null)
        {
            return cached;
        }

        var repository = await store.GetRepositoryAsync(owner, name, false, cancellationToken)
            ?? throw new NotFoundException($"Repository '{owner}/{name}' is not tracked.");

        var dto = repository.ToDto();
        await cache.SetAsync(key, dto, RepositoryCacheTtl, cancellationToken);
        return dto;
    }

    public async Task<IReadOnlyList<RepositoryDto>> ListAsync(CancellationToken cancellationToken)
    {
        var cached = await cache.GetAsync<List<RepositoryDto>>(CacheKeys.RepositoryList, cancellationToken);
        if (cached is not null)
        {
            return cached;
        }

        var repositories = await store.ListRepositoriesAsync(cancellationToken);
        var result = repositories.Select(x => x.ToDto()).ToList();
        await cache.SetAsync(CacheKeys.RepositoryList, result, RepositoryCacheTtl, cancellationToken);
        return result;
    }

    public async Task DeleteAsync(string owner, string name, CancellationToken cancellationToken)
    {
        ValidateRepositoryCoordinates(owner, name);
        var repository = await store.GetRepositoryAsync(owner, name, true, cancellationToken)
            ?? throw new NotFoundException($"Repository '{owner}/{name}' is not tracked.");

        await store.DeleteRepositoryAsync(repository, cancellationToken);
        await cache.RemoveAsync(CacheKeys.Repository(owner, name), cancellationToken);
        await cache.RemoveAsync(CacheKeys.RepositoryList, cancellationToken);
    }

    private static void ValidateRepositoryCoordinates(string owner, string name)
    {
        if (string.IsNullOrWhiteSpace(owner) || string.IsNullOrWhiteSpace(name))
        {
            throw new RequestValidationException("Both owner and repository name are required.");
        }
    }
}
