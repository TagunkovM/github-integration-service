using GitHubIntegration.Application.DTOs;
using GitHubIntegration.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GitHubIntegration.Api.Controllers;

[ApiController]
[Route("api/repositories")]
public sealed class RepositoriesController(
    RepositoryService repositoryService,
    SynchronizationService synchronizationService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<TrackRepositoryResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<TrackRepositoryResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<TrackRepositoryResponse>> Track(
        [FromBody] TrackRepositoryRequest request,
        CancellationToken cancellationToken)
    {
        var (repository, alreadyTracked) = await repositoryService.TrackAsync(request.Owner, request.Name, cancellationToken);
        if (alreadyTracked)
        {
            return Ok(new TrackRepositoryResponse(repository, null, true));
        }

        var sync = await synchronizationService.StartAsync(repository.Owner, repository.Name, cancellationToken);
        var response = new TrackRepositoryResponse(repository, sync.Id, false);
        return CreatedAtAction(nameof(Get), new { owner = repository.Owner, name = repository.Name }, response);
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<RepositoryDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<RepositoryDto>>> List(CancellationToken cancellationToken) =>
        Ok(await repositoryService.ListAsync(cancellationToken));

    [HttpGet("{owner}/{name}")]
    [ProducesResponseType<RepositoryDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<RepositoryDto>> Get(string owner, string name, CancellationToken cancellationToken) =>
        Ok(await repositoryService.GetAsync(owner, name, cancellationToken));

    [HttpDelete("{owner}/{name}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(string owner, string name, CancellationToken cancellationToken)
    {
        await repositoryService.DeleteAsync(owner, name, cancellationToken);
        return NoContent();
    }
}
