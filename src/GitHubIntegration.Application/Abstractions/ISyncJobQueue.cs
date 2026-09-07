namespace GitHubIntegration.Application.Abstractions;

public interface ISyncJobQueue
{
    ValueTask EnqueueAsync(Guid syncId, CancellationToken cancellationToken);
    ValueTask<Guid> DequeueAsync(CancellationToken cancellationToken);
}
