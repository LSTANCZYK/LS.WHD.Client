using LS.WHD.Client.Http;

namespace LS.WHD.Client.Tests;

/// <summary>
/// Tests for <see cref="QueryParameters"/> construction and usage.
/// </summary>
public class QueryParametersTests
{
    [Fact]
    public void Constructor_WithPairs_PopulatesCorrectly()
    {
        var qp = new QueryParameters(("limit", "10"), ("start", "20"));

        Assert.Equal("10", qp["limit"]);
        Assert.Equal("20", qp["start"]);
    }

    [Fact]
    public void DefaultConstructor_CreatesEmptyDictionary()
    {
        var qp = new QueryParameters();
        Assert.Empty(qp);
    }

    [Fact]
    public void CanAddAndOverwrite()
    {
        var qp = new QueryParameters(("search", "printer"));
        qp["search"] = "keyboard";

        Assert.Equal("keyboard", qp["search"]);
    }
}
