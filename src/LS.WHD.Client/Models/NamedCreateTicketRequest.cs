namespace LS.WHD.Client.Models;

/// <summary>
/// Payload for creating a new ticket where priority, request type, and custom fields
/// may be specified by display name rather than by numeric ID.
/// </summary>
/// <remarks>
/// Pass this model to <see cref="WhdClient.CreateTicketAsync"/> which uses the
/// <see cref="Metadata.IWhdMetadataCache"/> to translate names to IDs before
/// sending the request. Call <see cref="WhdClient.InitializeMetadataCacheAsync"/>
/// at least once before creating tickets with named values.
/// Numeric ID strings (e.g. <c>"2"</c>) are also accepted wherever a name is expected.
/// </remarks>
public sealed class NamedCreateTicketRequest
{
    /// <summary>Short description / subject of the ticket (required).</summary>
    public required string Subject { get; init; }

    /// <summary>Detailed description of the issue.</summary>
    public string? Detail { get; init; }

    /// <summary>ID of the client (end user) submitting or for whom the ticket is created.</summary>
    public long? ClientId { get; init; }

    /// <summary>
    /// Priority type identified by display name (e.g. <c>"High"</c>) or numeric ID string (e.g. <c>"2"</c>).
    /// Leave <c>null</c> to omit the priority.
    /// </summary>
    public string? PriorityType { get; init; }

    /// <summary>
    /// Request type / category identified by name (e.g. <c>"IT Support"</c>)
    /// or numeric ID string (e.g. <c>"10"</c>).
    /// Leave <c>null</c> to omit the request type.
    /// </summary>
    public string? RequestType { get; init; }

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
    /// Each entry identifies the field by label or numeric ID string;
    /// the metadata cache resolves labels to field definition IDs automatically.
    /// </summary>
    public List<NamedCustomFieldValue>? CustomFields { get; init; }
}
