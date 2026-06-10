using LS.WHD.Client.Models;

namespace LS.WHD.Client.Tests;

/// <summary>
/// Tests for <see cref="PaginationOptions"/> defaults.
/// </summary>
public class PaginationOptionsTests
{
    [Fact]
    public void Defaults_AreCorrect()
    {
        var opts = new PaginationOptions();
        Assert.Equal(25, opts.Limit);
        Assert.Equal(0, opts.Start);
    }

    [Fact]
    public void CustomValues_AreRespected()
    {
        var opts = new PaginationOptions { Limit = 50, Start = 100 };
        Assert.Equal(50, opts.Limit);
        Assert.Equal(100, opts.Start);
    }
}
