using LS.WHD.Client.Models;

namespace LS.WHD.Client.Services;

/// <summary>
/// Provides read operations for Web Help Desk status types and priority types.
/// </summary>
public interface ILookupService
{
    /// <summary>Returns all configured status types.</summary>
    Task<IReadOnlyList<StatusType>> GetStatusTypesAsync(
        CancellationToken cancellationToken = default);

    /// <summary>Returns all configured priority types.</summary>
    Task<IReadOnlyList<PriorityType>> GetPriorityTypesAsync(
        CancellationToken cancellationToken = default);

    /// <summary>Returns all configured technicians.</summary>
    Task<IReadOnlyList<Tech>> GetTechsAsync(
        PaginationOptions? pagination = null,
        CancellationToken cancellationToken = default);
}
