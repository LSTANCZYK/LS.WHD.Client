using LS.WHD.Client.Http;
using LS.WHD.Client.Models;

namespace LS.WHD.Client.Services;

internal sealed class LocationService : ILocationService
{
    private const string Resource = "Locations";
    private readonly WhdHttpClient _http;

    public LocationService(WhdHttpClient http) => _http = http;

    public async Task<IReadOnlyList<Location>> ListAsync(
        PaginationOptions? pagination = null,
        CancellationToken cancellationToken = default)
    {
        QueryParameters? qp = ServiceHelpers.BuildPagination(pagination);

        var result = await _http.GetAsync<List<Location>>(Resource, qp, cancellationToken);
        return result ?? [];
    }

    public Task<Location?> GetAsync(long id, CancellationToken cancellationToken = default)
        => _http.GetAsync<Location>($"{Resource}/{id}", cancellationToken: cancellationToken);
}
