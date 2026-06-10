using LS.WHD.Client.Exceptions;

namespace LS.WHD.Client.Tests;

/// <summary>
/// Tests for <see cref="WhdApiException"/>.
/// </summary>
public class WhdApiExceptionTests
{
    [Fact]
    public void Constructor_SetsStatusCodeAndMessage()
    {
        var ex = new WhdApiException(404, "Resource not found");

        Assert.Equal(404, ex.StatusCode);
        Assert.Equal("Resource not found", ex.Message);
    }

    [Fact]
    public void Constructor_WithInner_SetsInnerException()
    {
        var inner = new InvalidOperationException("inner");
        var ex = new WhdApiException(500, "Server error", inner);

        Assert.Equal(500, ex.StatusCode);
        Assert.Same(inner, ex.InnerException);
    }
}
