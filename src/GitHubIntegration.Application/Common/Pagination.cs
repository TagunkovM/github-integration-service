namespace GitHubIntegration.Application.Common;

public readonly record struct PageRequest(int Page, int PageSize);

public static class Pagination
{
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;

    public static PageRequest Normalize(int page, int pageSize)
    {
        var normalizedPage = page < 1 ? 1 : page;
        var normalizedPageSize = pageSize < 1 ? DefaultPageSize : Math.Min(pageSize, MaxPageSize);
        return new PageRequest(normalizedPage, normalizedPageSize);
    }
}
