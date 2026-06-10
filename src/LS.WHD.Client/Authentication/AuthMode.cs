namespace LS.WHD.Client.Authentication;

/// <summary>
/// Specifies the authentication mode used when connecting to the Web Help Desk API.
/// </summary>
public enum AuthMode
{
    /// <summary>
    /// Use an API key passed as a query parameter (<c>?apiKey=…</c>).
    /// This is the recommended mode for server-to-server integrations.
    /// </summary>
    ApiKey,

    /// <summary>
    /// Use HTTP Basic Authentication (username and password).
    /// </summary>
    BasicAuth
}
