using LS.WHD.Client.Models;

namespace LS.WHD.Client.Services;

/// <summary>
/// Provides read and search operations for Web Help Desk clients (end users / requesters).
/// </summary>
public interface IClientService
{
    /// <summary>Returns a paged list of clients.</summary>
    Task<IReadOnlyList<HelpDeskClient>> ListAsync(
        PaginationOptions? pagination = null,
        CancellationToken cancellationToken = default);

    /// <summary>Returns the client with the specified <paramref name="id"/>.</summary>
    Task<HelpDeskClient?> GetAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for clients whose username or email matches <paramref name="search"/>.
    /// </summary>
    Task<IReadOnlyList<HelpDeskClient>> SearchAsync(
        string search,
        PaginationOptions? pagination = null,
        CancellationToken cancellationToken = default);
}
