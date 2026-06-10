namespace LS.WHD.Client.Exceptions;

/// <summary>
/// Thrown when the SolarWinds Web Help Desk API returns a non-successful HTTP status code.
/// </summary>
public sealed class WhdApiException : Exception
{
    /// <summary>The HTTP status code returned by the API.</summary>
    public int StatusCode { get; }

    /// <inheritdoc cref="WhdApiException"/>
    public WhdApiException(int statusCode, string message)
        : base(message)
    {
        StatusCode = statusCode;
    }

    /// <inheritdoc cref="WhdApiException"/>
    public WhdApiException(int statusCode, string message, Exception inner)
        : base(message, inner)
    {
        StatusCode = statusCode;
    }
}
