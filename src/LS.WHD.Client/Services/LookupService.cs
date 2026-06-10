using LS.WHD.Client.Http;
using LS.WHD.Client.Models;

namespace LS.WHD.Client.Services;

internal sealed class LookupService : ILookupService
{
    private readonly WhdHttpClient _http;

    public LookupService(WhdHttpClient http) => _http = http;

    public async Task<IReadOnlyList<StatusType>> GetStatusTypesAsync(
        CancellationToken cancellationToken = default)
    {
        var result = await _http.GetAsync<List<StatusType>>("StatusTypes", cancellationToken: cancellationToken);
        return result ?? [];
    }

    public async Task<IReadOnlyList<PriorityType>> GetPriorityTypesAsync(
        CancellationToken cancellationToken = default)
    {
        var result = await _http.GetAsync<List<PriorityType>>("PriorityTypes", cancellationToken: cancellationToken);
        return result ?? [];
    }

    public async Task<IReadOnlyList<Tech>> GetTechsAsync(
        PaginationOptions? pagination = null,
        CancellationToken cancellationToken = default)
    {
        QueryParameters? qp = pagination is null ? null :
            new QueryParameters(("limit", pagination.Limit.ToString()), ("start", pagination.Start.ToString()));

        var result = await _http.GetAsync<List<Tech>>("Techs", qp, cancellationToken);
        return result ?? [];
    }
}
