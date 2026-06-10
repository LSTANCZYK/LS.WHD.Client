using LS.WHD.Client.Models;

namespace LS.WHD.Client.Services;

/// <summary>
/// Provides operations for Web Help Desk assets (hardware / software inventory).
/// </summary>
public interface IAssetService
{
    /// <summary>Returns a paged list of assets.</summary>
    Task<IReadOnlyList<Asset>> ListAsync(
        PaginationOptions? pagination = null,
        CancellationToken cancellationToken = default);

    /// <summary>Returns the asset with the specified <paramref name="id"/>.</summary>
    Task<Asset?> GetAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for assets whose name or tag matches <paramref name="search"/>.
    /// </summary>
    Task<IReadOnlyList<Asset>> SearchAsync(
        string search,
        PaginationOptions? pagination = null,
        CancellationToken cancellationToken = default);
}
