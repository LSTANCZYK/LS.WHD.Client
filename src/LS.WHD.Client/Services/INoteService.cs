using LS.WHD.Client.Models;

namespace LS.WHD.Client.Services;

/// <summary>
/// Provides operations for notes (journal entries / comments) on Web Help Desk tickets.
/// </summary>
public interface INoteService
{
    /// <summary>
    /// Returns all notes for the ticket with the given <paramref name="ticketId"/>.
    /// </summary>
    Task<IReadOnlyList<TicketNote>> ListByTicketAsync(
        long ticketId,
        CancellationToken cancellationToken = default);

    /// <summary>Returns the note with the specified <paramref name="noteId"/>.</summary>
    Task<TicketNote?> GetAsync(long noteId, CancellationToken cancellationToken = default);

    /// <summary>Adds a note to a ticket and returns the created note.</summary>
    Task<TicketNote?> CreateAsync(
        CreateNoteRequest request,
        CancellationToken cancellationToken = default);
}
