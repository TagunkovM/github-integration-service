namespace GitHubIntegration.Application.Common;

public static class CacheKeys
{
    public const string RepositoryList = "repositories:list";

    public static string Repository(string owner, string name) =>
        $"repository:{Normalize(owner)}/{Normalize(name)}";

    public static string Issues(string owner, string name, long version, int page, int pageSize, string? state, string? author) =>
        $"issues:{Normalize(owner)}/{Normalize(name)}:v{version}:p{page}:s{pageSize}:state={NormalizeOptional(state)}:author={NormalizeOptional(author)}";

    public static string PullRequests(string owner, string name, long version, int page, int pageSize, string? state, string? author) =>
        $"pulls:{Normalize(owner)}/{Normalize(name)}:v{version}:p{page}:s{pageSize}:state={NormalizeOptional(state)}:author={NormalizeOptional(author)}";

    public static string Commits(string owner, string name, long version, int page, int pageSize, string? branch) =>
        $"commits:{Normalize(owner)}/{Normalize(name)}:v{version}:p{page}:s{pageSize}:branch={NormalizeOptional(branch)}";

    private static string Normalize(string value) => value.Trim().ToLowerInvariant();
    private static string NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? "*" : Normalize(value);
}
