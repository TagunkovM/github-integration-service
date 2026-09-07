using GitHubIntegration.Application.Common;
using GitHubIntegration.Application.DTOs;
using GitHubIntegration.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GitHubIntegration.Api.Controllers;

[ApiController]
[Route("api/repositories/{owner}/{name}")]
public sealed class RepositoryDataController(RepositoryQueryService queryService) : ControllerBase
{
    [HttpGet("issues")]
    public async Task<ActionResult<PagedResult<IssueDto>>> GetIssues(
        string owner,
        string name,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? state = null,
        [FromQuery] string? author = null,
        CancellationToken cancellationToken = default) =>
        Ok(await queryService.GetIssuesAsync(owner, name, page, pageSize, state, author, cancellationToken));

    [HttpGet("pull-requests")]
    public async Task<ActionResult<PagedResult<PullRequestDto>>> GetPullRequests(
        string owner,
        string name,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? state = null,
        [FromQuery] string? author = null,
        CancellationToken cancellationToken = default) =>
        Ok(await queryService.GetPullRequestsAsync(owner, name, page, pageSize, state, author, cancellationToken));

    [HttpGet("commits")]
    public async Task<ActionResult<PagedResult<CommitDto>>> GetCommits(
        string owner,
        string name,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? branch = null,
        CancellationToken cancellationToken = default) =>
        Ok(await queryService.GetCommitsAsync(owner, name, page, pageSize, branch, cancellationToken));
}
