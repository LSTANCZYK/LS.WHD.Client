using System.Net;
using System.Text;
using System.Text.Json;
using System.Web;
using LS.WHD.Client.Authentication;
using LS.WHD.Client.Exceptions;
using LS.WHD.Client.Models;

namespace LS.WHD.Client.Http;

/// <summary>
/// Low-level HTTP wrapper that applies authentication, serialization,
/// and error handling for all WHD API calls.
/// </summary>
internal sealed class WhdHttpClient : IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    private readonly HttpClient _http;
    private readonly WhdClientOptions _options;

    public WhdHttpClient(WhdClientOptions options)
    {
        _options = options;
        _http = WhdHttpClientFactory.Create(options);
    }

    /// <summary>Initializes a new instance using a pre-configured <see cref="HttpClient"/> (useful for testing).</summary>
    internal WhdHttpClient(HttpClient httpClient, WhdClientOptions options)
    {
        _http = httpClient;
        _options = options;
    }

    // ──────────────────────── CRUD helpers ────────────────────────

    public async Task<T?> GetAsync<T>(
        string resource,
        QueryParameters? queryParams = null,
        CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(resource, queryParams);
        var response = await _http.GetAsync(url, cancellationToken);
        return await ReadResponseAsync<T>(response, cancellationToken);
    }

    public async Task<T?> PostAsync<T>(
        string resource,
        object body,
        CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(resource);
        var content = new StringContent(
            JsonSerializer.Serialize(body, JsonOptions), Encoding.UTF8, "application/json");
        var response = await _http.PostAsync(url, content, cancellationToken);
        return await ReadResponseAsync<T>(response, cancellationToken);
    }

    public async Task<T?> PutAsync<T>(
        string resource,
        object body,
        CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(resource);
        var content = new StringContent(
            JsonSerializer.Serialize(body, JsonOptions), Encoding.UTF8, "application/json");
        var response = await _http.PutAsync(url, content, cancellationToken);
        return await ReadResponseAsync<T>(response, cancellationToken);
    }

    public async Task DeleteAsync(
        string resource,
        CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(resource);
        var response = await _http.DeleteAsync(url, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
    }

    // ──────────────────────── Helpers ────────────────────────

    private string BuildUrl(string resource, QueryParameters? queryParams = null)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);

        if (_options.AuthMode == AuthMode.ApiKey && _options.ApiKey is not null)
            query["apiKey"] = _options.ApiKey;

        if (queryParams is not null)
        {
            foreach (var kv in queryParams)
                query[kv.Key] = kv.Value;
        }

        var qs = query.ToString();
        return qs?.Length > 0 ? $"{resource}?{qs}" : resource;
    }

    private static async Task<T?> ReadResponseAsync<T>(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        await EnsureSuccessAsync(response, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NoContent)
            return default;

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(json))
            return default;

        return JsonSerializer.Deserialize<T>(json, JsonOptions);
    }

    private static async Task EnsureSuccessAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
            return;

        var body = string.Empty;
        try { body = await response.Content.ReadAsStringAsync(cancellationToken); }
        catch { /* swallow read errors */ }

        throw new WhdApiException(
            (int)response.StatusCode,
            $"WHD API returned {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
    }

    public void Dispose() => _http.Dispose();
}
