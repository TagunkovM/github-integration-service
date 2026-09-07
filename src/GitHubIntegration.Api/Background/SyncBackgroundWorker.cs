using GitHubIntegration.Application.Abstractions;
using GitHubIntegration.Application.Services;

namespace GitHubIntegration.Api.Background;

public sealed class SyncBackgroundWorker(
    ISyncJobQueue queue,
    IServiceScopeFactory scopeFactory,
    ILogger<SyncBackgroundWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("GitHub synchronization worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            Guid syncId;
            try
            {
                syncId = await queue.DequeueAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }

            try
            {
                using var scope = scopeFactory.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<SynchronizationProcessor>();
                await processor.ProcessAsync(syncId, stoppingToken);
                logger.LogInformation("Synchronization {SyncId} completed.", syncId);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Synchronization {SyncId} failed.", syncId);
            }
        }

        logger.LogInformation("GitHub synchronization worker stopped.");
    }
}
