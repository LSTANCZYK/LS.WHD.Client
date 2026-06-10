namespace LS.WHD.Client.Models;

/// <summary>
/// Payload for updating an existing ticket via <c>PUT /Tickets/{id}</c>.
/// All properties are optional — only non-null values are serialized.
/// </summary>
public sealed class UpdateTicketRequest
{
    /// <summary>Updated subject / title of the ticket.</summary>
    public string? Subject { get; init; }

    /// <summary>Updated detailed description.</summary>
    public string? Detail { get; init; }

    /// <summary>ID of the new status type.</summary>
    public long? StatusTypeId { get; init; }

    /// <summary>ID of the new priority type.</summary>
    public long? PriorityTypeId { get; init; }

    /// <summary>ID of the new request type / category.</summary>
    public long? RequestTypeId { get; init; }

    /// <summary>ID of the new assigned technician.</summary>
    public long? TechId { get; init; }

    /// <summary>ID of the new location.</summary>
    public long? LocationId { get; init; }

    /// <summary>ID of the new linked asset.</summary>
    public long? AssetId { get; init; }

    /// <summary>Updated due date.</summary>
    public DateTimeOffset? DueDate { get; init; }

    /// <summary>
    /// Custom field values to update on the ticket.
    /// Each entry must specify the field <see cref="CustomField.Id"/> and a <see cref="CustomField.RestValue"/>.
    /// </summary>
    public List<CustomField>? CustomFields { get; init; }
}
