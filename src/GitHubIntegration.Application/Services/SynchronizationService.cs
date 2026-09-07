using GitHubIntegration.Application.Abstractions;
using GitHubIntegration.Application.DTOs;
using GitHubIntegration.Application.Exceptions;
using GitHubIntegration.Application.Mappings;
using GitHubIntegration.Domain.Entities;
using GitHubIntegration.Domain.Enums;

namespace GitHubIntegration.Application.Services;

public sealed class SynchronizationService(IIntegrationStore store, ISyncJobQueue queue)
{
    public async Task<SyncStatusDto> StartAsync(string owner, string name, CancellationToken cancellationToken)
    {
        var repository = await store.GetRepositoryAsync(owner, name, false, cancellationToken)
            ?? throw new NotFoundException($"Repository '{owner}/{name}' is not tracked.");

        var running = await store.GetRunningSyncAsync(repository.Id, cancellationToken);
        if (running is not null)
        {
            throw new ConflictException($"A synchronization for '{repository.FullName}' is already running (syncId: {running.Id}).");
        }

        var history = new SyncHistoryEntity
        {
            RepositoryId = repository.Id,
            StartedAt = DateTimeOffset.UtcNow,
            Status = SyncStatus.Running
        };

        await store.AddSyncHistoryAsync(history, cancellationToken);
        await queue.EnqueueAsync(history.Id, cancellationToken);

        return new SyncStatusDto(
            history.Id,
            repository.FullName,
            history.Status.ToString().ToLowerInvariant(),
            history.StartedAt,
            history.FinishedAt,
            history.SyncedIssues,
            history.SyncedPullRequests,
            history.SyncedCommits,
            history.ErrorMessage);
    }

    public async Task<SyncStatusDto> GetStatusAsync(Guid syncId, CancellationToken cancellationToken)
    {
        var history = await store.GetSyncHistoryAsync(syncId, false, cancellationToken)
            ?? throw new NotFoundException($"Synchronization '{syncId}' was not found.");

        return history.ToDto();
    }
}
