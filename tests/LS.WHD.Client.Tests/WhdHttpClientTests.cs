using System.Net;
using System.Text.Json;
using LS.WHD.Client.Exceptions;
using LS.WHD.Client.Http;
using LS.WHD.Client.Models;

namespace LS.WHD.Client.Tests;

/// <summary>
/// Tests for the core HTTP client layer: URL construction, auth, error handling.
/// </summary>
public class WhdHttpClientTests
{
    // ──────── helpers ────────

    private static (WhdHttpClient client, FakeHttpMessageHandler handler) BuildClient(
        WhdClientOptions options,
        HttpStatusCode statusCode,
        string responseBody = "[]")
    {
        var handler = new FakeHttpMessageHandler(statusCode, responseBody);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(options.BaseUrl.TrimEnd('/') + WhdHttpClientFactory.ApiBasePath)
        };
        var client = new WhdHttpClient(httpClient, options);
        return (client, handler);
    }

    // ──────── API key appended to query string ────────

    [Fact]
    public async Task GetAsync_ApiKeyMode_AppendsApiKeyToQueryString()
    {
        var options = new WhdClientOptions
        {
            BaseUrl = "https://whd.example.com",
            AuthMode = Authentication.AuthMode.ApiKey,
            ApiKey = "test-api-key"
        };

        var (client, handler) = BuildClient(options, HttpStatusCode.OK, "[]");

        await client.GetAsync<List<Ticket>>("Tickets");

        Assert.NotNull(handler.LastRequest);
        Assert.Contains("apiKey=test-api-key", handler.LastRequest!.RequestUri!.Query);
    }

    [Fact]
    public async Task GetAsync_ApiKeyMode_WithAdditionalParams_AppendsAll()
    {
        var options = new WhdClientOptions
        {
            BaseUrl = "https://whd.example.com",
            AuthMode = Authentication.AuthMode.ApiKey,
            ApiKey = "mykey"
        };

        var (client, handler) = BuildClient(options, HttpStatusCode.OK, "[]");

        await client.GetAsync<List<Ticket>>("Tickets",
            new QueryParameters(("limit", "10"), ("start", "0")));

        Assert.NotNull(handler.LastRequest);
        var query = handler.LastRequest!.RequestUri!.Query;
        Assert.Contains("apiKey=mykey", query);
        Assert.Contains("limit=10", query);
        Assert.Contains("start=0", query);
    }

    // ──────── Error handling ────────

    [Theory]
    [InlineData(HttpStatusCode.NotFound, 404)]
    [InlineData(HttpStatusCode.Unauthorized, 401)]
    [InlineData(HttpStatusCode.InternalServerError, 500)]
    public async Task GetAsync_ErrorStatusCode_ThrowsWhdApiException(
        HttpStatusCode statusCode, int expectedCode)
    {
        var options = new WhdClientOptions
        {
            BaseUrl = "https://whd.example.com",
            ApiKey = "key"
        };

        var (client, _) = BuildClient(options, statusCode, "Error");

        var ex = await Assert.ThrowsAsync<WhdApiException>(
            () => client.GetAsync<Ticket>("Tickets/1"));

        Assert.Equal(expectedCode, ex.StatusCode);
    }

    // ──────── Deserialization ────────

    [Fact]
    public async Task GetAsync_ValidJson_DeserializesCorrectly()
    {
        const string json = """[{"id":1,"subject":"Test Ticket"}]""";

        var options = new WhdClientOptions
        {
            BaseUrl = "https://whd.example.com",
            ApiKey = "key"
        };

        var (client, _) = BuildClient(options, HttpStatusCode.OK, json);

        var result = await client.GetAsync<List<Ticket>>("Tickets");

        Assert.NotNull(result);
        Assert.Single(result!);
        Assert.Equal(1, result![0].Id);
        Assert.Equal("Test Ticket", result![0].Subject);
    }

    [Fact]
    public async Task GetAsync_EmptyBody_ReturnsDefault()
    {
        var options = new WhdClientOptions
        {
            BaseUrl = "https://whd.example.com",
            ApiKey = "key"
        };

        var (client, _) = BuildClient(options, HttpStatusCode.NoContent, string.Empty);

        var result = await client.GetAsync<Ticket>("Tickets/1");
        Assert.Null(result);
    }

    // ──────── POST / PUT ────────

    [Fact]
    public async Task PostAsync_SendsJsonBody()
    {
        const string responseJson = """{"id":100,"subject":"New Ticket"}""";

        var options = new WhdClientOptions
        {
            BaseUrl = "https://whd.example.com",
            ApiKey = "key"
        };

        var (client, handler) = BuildClient(options, HttpStatusCode.Created, responseJson);

        var request = new CreateTicketRequest { Subject = "New Ticket", ClientId = 1 };
        var result = await client.PostAsync<Ticket>("Tickets", request);

        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);

        var sentBody = await handler.LastRequest.Content!.ReadAsStringAsync();
        Assert.Contains("New Ticket", sentBody);

        Assert.NotNull(result);
        Assert.Equal(100, result!.Id);
    }

    [Fact]
    public async Task DeleteAsync_CallsDeleteMethod()
    {
        var options = new WhdClientOptions
        {
            BaseUrl = "https://whd.example.com",
            ApiKey = "key"
        };

        var (client, handler) = BuildClient(options, HttpStatusCode.NoContent, string.Empty);

        await client.DeleteAsync("Tickets/42");

        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Delete, handler.LastRequest!.Method);
        Assert.Contains("Tickets/42", handler.LastRequest.RequestUri!.ToString());
    }
}

// ──────── Fake HTTP handler for testing ────────

/// <summary>
/// A test double for <see cref="HttpMessageHandler"/> that returns a pre-configured response.
/// </summary>
internal sealed class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly HttpStatusCode _statusCode;
    private readonly string _responseBody;

    public HttpRequestMessage? LastRequest { get; private set; }

    public FakeHttpMessageHandler(HttpStatusCode statusCode, string responseBody)
    {
        _statusCode = statusCode;
        _responseBody = responseBody;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        LastRequest = request;
        var response = new HttpResponseMessage(_statusCode)
        {
            Content = new StringContent(_responseBody, System.Text.Encoding.UTF8, "application/json")
        };
        return Task.FromResult(response);
    }
}
