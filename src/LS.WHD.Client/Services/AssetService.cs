using LS.WHD.Client.Http;
using LS.WHD.Client.Models;

namespace LS.WHD.Client.Services;

internal sealed class AssetService : IAssetService
{
    private const string Resource = "Assets";
    private readonly WhdHttpClient _http;

    public AssetService(WhdHttpClient http) => _http = http;

    public async Task<IReadOnlyList<Asset>> ListAsync(
        PaginationOptions? pagination = null,
        CancellationToken cancellationToken = default)
    {
        var qp = BuildQuery(pagination);
        var result = await _http.GetAsync<List<Asset>>(Resource, qp, cancellationToken);
        return result ?? [];
    }

    public Task<Asset?> GetAsync(long id, CancellationToken cancellationToken = default)
        => _http.GetAsync<Asset>($"{Resource}/{id}", cancellationToken: cancellationToken);

    public async Task<IReadOnlyList<Asset>> SearchAsync(
        string search,
        PaginationOptions? pagination = null,
        CancellationToken cancellationToken = default)
    {
        var qp = BuildQuery(pagination);
        qp ??= new QueryParameters();
        qp["search"] = search;

        var result = await _http.GetAsync<List<Asset>>(Resource, qp, cancellationToken);
        return result ?? [];
    }

    private static QueryParameters? BuildQuery(PaginationOptions? p)
    {
        if (p is null) return null;
        return new QueryParameters(("limit", p.Limit.ToString()), ("start", p.Start.ToString()));
    }
}
