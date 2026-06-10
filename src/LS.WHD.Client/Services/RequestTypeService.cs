using LS.WHD.Client.Http;
using LS.WHD.Client.Models;

namespace LS.WHD.Client.Services;

internal sealed class RequestTypeService : IRequestTypeService
{
    private const string Resource = "RequestTypes";
    private readonly WhdHttpClient _http;

    public RequestTypeService(WhdHttpClient http) => _http = http;

    public async Task<IReadOnlyList<RequestType>> ListAsync(
        PaginationOptions? pagination = null,
        CancellationToken cancellationToken = default)
    {
        QueryParameters? qp = ServiceHelpers.BuildPagination(pagination);

        var result = await _http.GetAsync<List<RequestType>>(Resource, qp, cancellationToken);
        return result ?? [];
    }

    public Task<RequestType?> GetAsync(long id, CancellationToken cancellationToken = default)
        => _http.GetAsync<RequestType>($"{Resource}/{id}", cancellationToken: cancellationToken);
}
