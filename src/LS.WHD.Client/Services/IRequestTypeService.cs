using LS.WHD.Client.Models;

namespace LS.WHD.Client.Services;

/// <summary>
/// Provides read operations for Web Help Desk request types (categories).
/// </summary>
public interface IRequestTypeService
{
    /// <summary>Returns a paged list of request types.</summary>
    Task<IReadOnlyList<RequestType>> ListAsync(
        PaginationOptions? pagination = null,
        CancellationToken cancellationToken = default);

    /// <summary>Returns the request type with the specified <paramref name="id"/>.</summary>
    Task<RequestType?> GetAsync(long id, CancellationToken cancellationToken = default);
}
