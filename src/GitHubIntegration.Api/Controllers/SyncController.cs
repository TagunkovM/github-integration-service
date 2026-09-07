using GitHubIntegration.Application.DTOs;
using GitHubIntegration.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GitHubIntegration.Api.Controllers;

[ApiController]
[Route("api")]
public sealed class SyncController(SynchronizationService synchronizationService) : ControllerBase
{
    [HttpPost("repositories/{owner}/{name}/sync")]
    [ProducesResponseType<SyncStatusDto>(StatusCodes.Status202Accepted)]
    public async Task<ActionResult<SyncStatusDto>> Start(
        string owner,
        string name,
        CancellationToken cancellationToken)
    {
        var result = await synchronizationService.StartAsync(owner, name, cancellationToken);
        return Accepted($"/api/sync/{result.Id}", result);
    }

    [HttpGet("sync/{syncId:guid}")]
    [ProducesResponseType<SyncStatusDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<SyncStatusDto>> GetStatus(Guid syncId, CancellationToken cancellationToken) =>
        Ok(await synchronizationService.GetStatusAsync(syncId, cancellationToken));
}
