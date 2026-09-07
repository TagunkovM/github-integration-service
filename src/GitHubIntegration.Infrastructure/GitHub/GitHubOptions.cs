namespace GitHubIntegration.Infrastructure.GitHub;

public sealed class GitHubOptions
{
    public const string SectionName = "GitHub";

    public string BaseUrl { get; init; } = "https://api.github.com/";
    public string Token { get; init; } = string.Empty;
    public string ApiVersion { get; init; } = "2026-03-10";
    public int PageSize { get; init; } = 100;
    public int MaxPages { get; init; } = 2;
}
