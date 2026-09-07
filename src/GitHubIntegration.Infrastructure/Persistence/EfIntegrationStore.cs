using GitHubIntegration.Application.Abstractions;
using GitHubIntegration.Application.Common;
using GitHubIntegration.Domain.Entities;
using GitHubIntegration.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GitHubIntegration.Infrastructure.Persistence;

public sealed class EfIntegrationStore(AppDbContext dbContext) : IIntegrationStore
{
    public async Task<RepositoryEntity?> GetRepositoryAsync(string owner, string name, bool tracking, CancellationToken cancellationToken)
    {
        IQueryable<RepositoryEntity> query = dbContext.Repositories;
        if (!tracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(
            x => EF.Functions.ILike(x.Owner, owner.Trim()) && EF.Functions.ILike(x.Name, name.Trim()),
            cancellationToken);
    }

    public async Task<RepositoryEntity?> GetRepositoryByIdAsync(Guid id, bool tracking, CancellationToken cancellationToken)
    {
        IQueryable<RepositoryEntity> query = dbContext.Repositories;
        if (!tracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<RepositoryEntity>> ListRepositoriesAsync(CancellationToken cancellationToken) =>
        await dbContext.Repositories.AsNoTracking().OrderBy(x => x.FullName).ToListAsync(cancellationToken);

    public async Task AddRepositoryAsync(RepositoryEntity repository, CancellationToken cancellationToken)
    {
        dbContext.Repositories.Add(repository);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteRepositoryAsync(RepositoryEntity repository, CancellationToken cancellationToken)
    {
        dbContext.Repositories.Remove(repository);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);

    public async Task<PagedResult<IssueEntity>> GetIssuesAsync(
        Guid repositoryId,
        int page,
        int pageSize,
        IssueState? state,
        string? author,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Issues.AsNoTracking().Where(x => x.RepositoryId == repositoryId);
        if (state.HasValue)
        {
            query = query.Where(x => x.State == state.Value);
        }
        if (!string.IsNullOrWhiteSpace(author))
        {
            query = query.Where(x => x.AuthorLogin != null && EF.Functions.ILike(x.AuthorLogin, author.Trim()));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderByDescending(x => x.UpdatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<IssueEntity>(items, page, pageSize, total);
    }

    public async Task<PagedResult<PullRequestEntity>> GetPullRequestsAsync(
        Guid repositoryId,
        int page,
        int pageSize,
        PullRequestState? state,
        string? author,
        CancellationToken cancellationToken)
    {
        var query = dbContext.PullRequests.AsNoTracking().Where(x => x.RepositoryId == repositoryId);
        if (state.HasValue)
        {
            query = query.Where(x => x.State == state.Value);
        }
        if (!string.IsNullOrWhiteSpace(author))
        {
            query = query.Where(x => x.AuthorLogin != null && EF.Functions.ILike(x.AuthorLogin, author.Trim()));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderByDescending(x => x.UpdatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<PullRequestEntity>(items, page, pageSize, total);
    }

    public async Task<PagedResult<CommitEntity>> GetCommitsAsync(
        Guid repositoryId,
        int page,
        int pageSize,
        string? branch,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Commits.AsNoTracking().Where(x => x.RepositoryId == repositoryId);
        if (!string.IsNullOrWhiteSpace(branch))
        {
            query = query.Where(x => EF.Functions.ILike(x.Branch, branch.Trim()));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderByDescending(x => x.CommittedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<CommitEntity>(items, page, pageSize, total);
    }

    public async Task UpsertIssuesAsync(Guid repositoryId, IReadOnlyCollection<IssueEntity> issues, CancellationToken cancellationToken)
    {
        if (issues.Count == 0)
        {
            return;
        }

        var numbers = issues.Select(x => x.Number).Distinct().ToArray();
        var existing = await dbContext.Issues
            .Where(x => x.RepositoryId == repositoryId && numbers.Contains(x.Number))
            .ToDictionaryAsync(x => x.Number, cancellationToken);

        foreach (var incoming in issues)
        {
            if (existing.TryGetValue(incoming.Number, out var current))
            {
                current.GitHubId = incoming.GitHubId;
                current.Title = incoming.Title;
                current.Body = incoming.Body;
                current.State = incoming.State;
                current.AuthorLogin = incoming.AuthorLogin;
                current.CommentsCount = incoming.CommentsCount;
                current.CreatedAt = incoming.CreatedAt;
                current.UpdatedAt = incoming.UpdatedAt;
                current.ClosedAt = incoming.ClosedAt;
            }
            else
            {
                dbContext.Issues.Add(incoming);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpsertPullRequestsAsync(Guid repositoryId, IReadOnlyCollection<PullRequestEntity> pullRequests, CancellationToken cancellationToken)
    {
        if (pullRequests.Count == 0)
        {
            return;
        }

        var numbers = pullRequests.Select(x => x.Number).Distinct().ToArray();
        var existing = await dbContext.PullRequests
            .Where(x => x.RepositoryId == repositoryId && numbers.Contains(x.Number))
            .ToDictionaryAsync(x => x.Number, cancellationToken);

        foreach (var incoming in pullRequests)
        {
            if (existing.TryGetValue(incoming.Number, out var current))
            {
                current.GitHubId = incoming.GitHubId;
                current.Title = incoming.Title;
                current.Body = incoming.Body;
                current.State = incoming.State;
                current.AuthorLogin = incoming.AuthorLogin;
                current.SourceBranch = incoming.SourceBranch;
                current.TargetBranch = incoming.TargetBranch;
                current.IsDraft = incoming.IsDraft;
                current.IsMerged = incoming.IsMerged;
                current.CreatedAt = incoming.CreatedAt;
                current.UpdatedAt = incoming.UpdatedAt;
                current.MergedAt = incoming.MergedAt;
                current.ClosedAt = incoming.ClosedAt;
            }
            else
            {
                dbContext.PullRequests.Add(incoming);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpsertCommitsAsync(Guid repositoryId, IReadOnlyCollection<CommitEntity> commits, CancellationToken cancellationToken)
    {
        if (commits.Count == 0)
        {
            return;
        }

        var shas = commits.Select(x => x.Sha).Distinct().ToArray();
        var existingItems = await dbContext.Commits
            .Where(x => x.RepositoryId == repositoryId && shas.Contains(x.Sha))
            .ToListAsync(cancellationToken);
        var existing = existingItems.ToDictionary(x => x.Sha, StringComparer.OrdinalIgnoreCase);

        foreach (var incoming in commits)
        {
            if (existing.TryGetValue(incoming.Sha, out var current))
            {
                current.Branch = incoming.Branch;
                current.Message = incoming.Message;
                current.AuthorName = incoming.AuthorName;
                current.AuthorEmail = incoming.AuthorEmail;
                current.AuthorLogin = incoming.AuthorLogin;
                current.CommittedAt = incoming.CommittedAt;
                current.IsVerified = incoming.IsVerified;
            }
            else
            {
                dbContext.Commits.Add(incoming);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddSyncHistoryAsync(SyncHistoryEntity syncHistory, CancellationToken cancellationToken)
    {
        dbContext.SyncHistory.Add(syncHistory);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<SyncHistoryEntity?> GetSyncHistoryAsync(Guid syncId, bool tracking, CancellationToken cancellationToken)
    {
        IQueryable<SyncHistoryEntity> query = dbContext.SyncHistory.Include(x => x.Repository);
        if (!tracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(x => x.Id == syncId, cancellationToken);
    }

    public async Task<SyncHistoryEntity?> GetRunningSyncAsync(Guid repositoryId, CancellationToken cancellationToken) =>
        await dbContext.SyncHistory.AsNoTracking()
            .Where(x => x.RepositoryId == repositoryId && x.Status == SyncStatus.Running)
            .OrderByDescending(x => x.StartedAt)
            .FirstOrDefaultAsync(cancellationToken);
}
