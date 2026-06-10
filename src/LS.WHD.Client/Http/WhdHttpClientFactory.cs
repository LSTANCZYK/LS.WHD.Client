using System.Net.Http.Headers;
using System.Text;
using LS.WHD.Client.Authentication;

namespace LS.WHD.Client.Http;

/// <summary>
/// Builds and configures an <see cref="HttpClient"/> suitable for communicating
/// with the SolarWinds Web Help Desk REST API.
/// </summary>
internal static class WhdHttpClientFactory
{
    internal const string ApiBasePath = "/helpdesk/WebObjects/HELPDESK.woa/ra/";

    /// <summary>
    /// Creates an <see cref="HttpClient"/> configured for the given options.
    /// The caller is responsible for disposing the returned client.
    /// </summary>
    public static HttpClient Create(WhdClientOptions options)
    {
        HttpMessageHandler handler = options.IgnoreSslErrors
            ? new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            }
            : new HttpClientHandler();

        var client = new HttpClient(handler, disposeHandler: true)
        {
            BaseAddress = new Uri(options.BaseUrl.TrimEnd('/') + ApiBasePath),
            Timeout = options.Timeout
        };

        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        if (options.AuthMode == AuthMode.BasicAuth &&
            options.Username is not null &&
            options.Password is not null)
        {
            var credentials = Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{options.Username}:{options.Password}"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", credentials);
        }

        return client;
    }
}
