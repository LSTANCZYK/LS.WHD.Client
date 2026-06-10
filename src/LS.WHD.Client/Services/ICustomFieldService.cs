using LS.WHD.Client.Models;

namespace LS.WHD.Client.Services;

/// <summary>
/// Provides read operations for Web Help Desk custom field definitions.
/// </summary>
public interface ICustomFieldService
{
    /// <summary>
    /// Returns all custom field definitions configured in Web Help Desk.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<IReadOnlyList<CustomFieldDefinition>> ListAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the custom field definition with the specified <paramref name="id"/>.
    /// </summary>
    Task<CustomFieldDefinition?> GetAsync(long id, CancellationToken cancellationToken = default);
}
