using LS.WHD.Client.Http;
using LS.WHD.Client.Models;

namespace LS.WHD.Client.Services;

internal sealed class NoteService : INoteService
{
    private const string Resource = "TicketNotes";
    private readonly WhdHttpClient _http;

    public NoteService(WhdHttpClient http) => _http = http;

    public async Task<IReadOnlyList<TicketNote>> ListByTicketAsync(
        long ticketId,
        CancellationToken cancellationToken = default)
    {
        var qp = new QueryParameters(("ticketId", ticketId.ToString()));
        var result = await _http.GetAsync<List<TicketNote>>(Resource, qp, cancellationToken);
        return result ?? [];
    }

    public Task<TicketNote?> GetAsync(long noteId, CancellationToken cancellationToken = default)
        => _http.GetAsync<TicketNote>($"{Resource}/{noteId}", cancellationToken: cancellationToken);

    public Task<TicketNote?> CreateAsync(CreateNoteRequest request, CancellationToken cancellationToken = default)
        => _http.PostAsync<TicketNote>(Resource, request, cancellationToken);
}
