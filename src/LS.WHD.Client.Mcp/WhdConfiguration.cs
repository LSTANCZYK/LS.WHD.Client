using LS.WHD.Client.Authentication;

namespace LS.WHD.Client.Mcp;

/// <summary>
/// Reads MCP server configuration from environment variables and constructs a
/// <see cref="WhdClient"/> instance.
/// </summary>
/// <remarks>
/// Supported environment variables:
/// <list type="table">
///   <item><term>WHD_BASE_URL</term><description>Base URL of the Web Help Desk server (required).</description></item>
///   <item><term>WHD_AUTH_MODE</term><description><c>ApiKey</c> (default) or <c>BasicAuth</c>.</description></item>
///   <item><term>WHD_API_KEY</term><description>API key when using ApiKey auth mode.</description></item>
///   <item><term>WHD_USERNAME</term><description>Username when using BasicAuth mode.</description></item>
///   <item><term>WHD_PASSWORD</term><description>Password when using BasicAuth mode.</description></item>
///   <item><term>WHD_IGNORE_SSL_ERRORS</term><description><c>true</c> to skip TLS validation (dev only).</description></item>
/// </list>
/// </remarks>
internal static class WhdConfiguration
{
    public static WhdClientOptions LoadOptions()
    {
        var baseUrl = Environment.GetEnvironmentVariable("WHD_BASE_URL")
            ?? throw new InvalidOperationException(
                "Required environment variable 'WHD_BASE_URL' is not set.");

        var authModeRaw = Environment.GetEnvironmentVariable("WHD_AUTH_MODE") ?? "ApiKey";
        var authMode = Enum.TryParse<AuthMode>(authModeRaw, ignoreCase: true, out var parsed)
            ? parsed
            : throw new InvalidOperationException(
                $"Invalid WHD_AUTH_MODE '{authModeRaw}'. Valid values: ApiKey, BasicAuth.");

        var ignoreSslRaw = Environment.GetEnvironmentVariable("WHD_IGNORE_SSL_ERRORS") ?? "false";
        var ignoreSsl = bool.TryParse(ignoreSslRaw, out var ssl) && ssl;

        return new WhdClientOptions
        {
            BaseUrl        = baseUrl,
            AuthMode       = authMode,
            ApiKey         = Environment.GetEnvironmentVariable("WHD_API_KEY"),
            Username       = Environment.GetEnvironmentVariable("WHD_USERNAME"),
            Password       = Environment.GetEnvironmentVariable("WHD_PASSWORD"),
            IgnoreSslErrors = ignoreSsl
        };
    }
}
