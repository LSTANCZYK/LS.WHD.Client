namespace LS.WHD.Client.Models;

/// <summary>
/// Payload for creating a new ticket via <c>POST /Tickets</c>.
/// </summary>
public sealed class CreateTicketRequest
{
    /// <summary>Short description / subject of the ticket (required).</summary>
    public required string Subject { get; init; }

    /// <summary>Detailed description of the issue.</summary>
    public string? Detail { get; init; }

    /// <summary>ID of the client (end user) submitting or for whom the ticket is created.</summary>
    public long? ClientId { get; init; }

    /// <summary>ID of the request type / category.</summary>
    public long? RequestTypeId { get; init; }

    /// <summary>ID of the priority type.</summary>
    public long? PriorityTypeId { get; init; }

    /// <summary>ID of the location to associate with the ticket.</summary>
    public long? LocationId { get; init; }

    /// <summary>ID of the asset to link to the ticket.</summary>
    public long? AssetId { get; init; }

    /// <summary>ID of the technician to assign to the ticket.</summary>
    public long? TechId { get; init; }

    /// <summary>Due date for resolving the ticket.</summary>
    public DateTimeOffset? DueDate { get; init; }

    /// <summary>
    /// Custom field values to set on the ticket.
    /// Each entry must specify the field <see cref="CustomField.Id"/> and a <see cref="CustomField.RestValue"/>.
    /// </summary>
    public List<CustomField>? CustomFields { get; init; }
}
