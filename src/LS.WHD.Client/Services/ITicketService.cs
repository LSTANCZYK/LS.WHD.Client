using LS.WHD.Client.Models;

namespace LS.WHD.Client.Services;

/// <summary>
/// Provides CRUD operations for Web Help Desk tickets.
/// </summary>
public interface ITicketService
{
    /// <summary>
    /// Returns a paged list of tickets.
    /// </summary>
    /// <param name="pagination">Optional pagination settings.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<IReadOnlyList<Ticket>> ListAsync(
        PaginationOptions? pagination = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the ticket with the specified <paramref name="id"/>.
    /// </summary>
    Task<Ticket?> GetAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new ticket and returns the created entity.
    /// </summary>
    Task<Ticket?> CreateAsync(
        CreateTicketRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing ticket and returns the updated entity.
    /// </summary>
    Task<Ticket?> UpdateAsync(
        long id,
        UpdateTicketRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the ticket with the specified <paramref name="id"/>.
    /// </summary>
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
