namespace LS.WHD.Client;

/// <summary>
/// Configuration options for <see cref="WhdClient"/>.
/// </summary>
public sealed class WhdClientOptions
{
    /// <summary>
    /// The base URL of the Web Help Desk server, e.g.
    /// <c>https://helpdesk.example.com:8081</c>.
    /// Do not include a trailing slash or the API path.
    /// </summary>
    public required string BaseUrl { get; init; }

    /// <summary>
    /// Authentication mode. Defaults to <see cref="Authentication.AuthMode.ApiKey"/>.
    /// </summary>
    public Authentication.AuthMode AuthMode { get; init; } = Authentication.AuthMode.ApiKey;

    /// <summary>
    /// API key for the technician account, used when
    /// <see cref="AuthMode"/> is <see cref="Authentication.AuthMode.ApiKey"/>.
    /// </summary>
    public string? ApiKey { get; init; }

    /// <summary>
    /// Username for Basic Auth, used when
    /// <see cref="AuthMode"/> is <see cref="Authentication.AuthMode.BasicAuth"/>.
    /// </summary>
    public string? Username { get; init; }

    /// <summary>
    /// Password for Basic Auth, used when
    /// <see cref="AuthMode"/> is <see cref="Authentication.AuthMode.BasicAuth"/>.
    /// </summary>
    public string? Password { get; init; }

    /// <summary>
    /// Whether to skip SSL certificate validation. Set to <c>true</c> only in
    /// development/test environments against self-signed certificates.
    /// </summary>
    public bool IgnoreSslErrors { get; init; } = false;

    /// <summary>
    /// Timeout for individual HTTP requests. Defaults to 30 seconds.
    /// </summary>
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(30);
}
