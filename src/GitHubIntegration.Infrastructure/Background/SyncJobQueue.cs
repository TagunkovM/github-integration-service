using System.Threading.Channels;
using GitHubIntegration.Application.Abstractions;

namespace GitHubIntegration.Infrastructure.Background;

public sealed class SyncJobQueue : ISyncJobQueue
{
    private readonly Channel<Guid> _queue = Channel.CreateBounded<Guid>(new BoundedChannelOptions(100)
    {
        FullMode = BoundedChannelFullMode.Wait,
        SingleReader = true,
        SingleWriter = false
    });

    public ValueTask EnqueueAsync(Guid syncId, CancellationToken cancellationToken) =>
        _queue.Writer.WriteAsync(syncId, cancellationToken);

    public ValueTask<Guid> DequeueAsync(CancellationToken cancellationToken) =>
        _queue.Reader.ReadAsync(cancellationToken);
}
