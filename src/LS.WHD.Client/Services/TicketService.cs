using LS.WHD.Client.Http;
using LS.WHD.Client.Models;

namespace LS.WHD.Client.Services;

internal sealed class TicketService : ITicketService
{
    private const string Resource = "Tickets";
    private readonly WhdHttpClient _http;

    public TicketService(WhdHttpClient http) => _http = http;

    public async Task<IReadOnlyList<Ticket>> ListAsync(
        PaginationOptions? pagination = null,
        CancellationToken cancellationToken = default)
    {
        var qp = BuildPagination(pagination);
        var result = await _http.GetAsync<List<Ticket>>(Resource, qp, cancellationToken);
        return result ?? [];
    }

    public Task<Ticket?> GetAsync(long id, CancellationToken cancellationToken = default)
        => _http.GetAsync<Ticket>($"{Resource}/{id}", cancellationToken: cancellationToken);

    public Task<Ticket?> CreateAsync(CreateTicketRequest request, CancellationToken cancellationToken = default)
        => _http.PostAsync<Ticket>(Resource, request, cancellationToken);

    public Task<Ticket?> UpdateAsync(long id, UpdateTicketRequest request, CancellationToken cancellationToken = default)
        => _http.PutAsync<Ticket>($"{Resource}/{id}", request, cancellationToken);

    public Task DeleteAsync(long id, CancellationToken cancellationToken = default)
        => _http.DeleteAsync($"{Resource}/{id}", cancellationToken);

    private static QueryParameters? BuildPagination(PaginationOptions? p)
    {
        if (p is null) return null;
        return new QueryParameters(("limit", p.Limit.ToString()), ("start", p.Start.ToString()));
    }
}
