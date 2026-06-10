using LS.WHD.Client.Models;

namespace LS.WHD.Client.Services;

/// <summary>
/// Provides read operations for Web Help Desk locations.
/// </summary>
public interface ILocationService
{
    /// <summary>Returns a paged list of locations.</summary>
    Task<IReadOnlyList<Location>> ListAsync(
        PaginationOptions? pagination = null,
        CancellationToken cancellationToken = default);

    /// <summary>Returns the location with the specified <paramref name="id"/>.</summary>
    Task<Location?> GetAsync(long id, CancellationToken cancellationToken = default);
}
