using Xunit;
using GitHubIntegration.Application.Common;

namespace GitHubIntegration.UnitTests.Common;

public sealed class PaginationTests
{
    [Theory]
    [InlineData(0, 0, 1, 20)]
    [InlineData(-10, -5, 1, 20)]
    [InlineData(2, 500, 2, 100)]
    [InlineData(3, 50, 3, 50)]
    public void Normalize_ClampsInvalidValues(int page, int pageSize, int expectedPage, int expectedPageSize)
    {
        var result = Pagination.Normalize(page, pageSize);

        Assert.Equal(expectedPage, result.Page);
        Assert.Equal(expectedPageSize, result.PageSize);
    }
}
