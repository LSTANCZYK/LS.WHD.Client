using LS.WHD.Client.Http;
using LS.WHD.Client.Models;

namespace LS.WHD.Client.Services;

internal sealed class ClientService : IClientService
{
    private const string Resource = "Clients";
    private readonly WhdHttpClient _http;

    public ClientService(WhdHttpClient http) => _http = http;

    public async Task<IReadOnlyList<HelpDeskClient>> ListAsync(
        PaginationOptions? pagination = null,
        CancellationToken cancellationToken = default)
    {
        var qp = ServiceHelpers.BuildPagination(pagination);
        var result = await _http.GetAsync<List<HelpDeskClient>>(Resource, qp, cancellationToken);
        return result ?? [];
    }

    public Task<HelpDeskClient?> GetAsync(long id, CancellationToken cancellationToken = default)
        => _http.GetAsync<HelpDeskClient>($"{Resource}/{id}", cancellationToken: cancellationToken);

    public async Task<IReadOnlyList<HelpDeskClient>> SearchAsync(
        string search,
        PaginationOptions? pagination = null,
        CancellationToken cancellationToken = default)
    {
        var qp = ServiceHelpers.BuildPagination(pagination);
        qp ??= new QueryParameters();
        qp["search"] = search;

        var result = await _http.GetAsync<List<HelpDeskClient>>(Resource, qp, cancellationToken);
        return result ?? [];
    }
}
