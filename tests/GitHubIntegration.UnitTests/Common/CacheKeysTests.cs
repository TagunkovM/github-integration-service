using Xunit;
using GitHubIntegration.Application.Common;

namespace GitHubIntegration.UnitTests.Common;

public sealed class CacheKeysTests
{
    [Fact]
    public void Issues_ChangesWhenSyncVersionChanges()
    {
        var first = CacheKeys.Issues("dotnet", "runtime", 100, 1, 20, "open", null);
        var second = CacheKeys.Issues("dotnet", "runtime", 200, 1, 20, "open", null);

        Assert.NotEqual(first, second);
        Assert.Contains("dotnet/runtime", first);
    }
}
