namespace LS.WHD.Client.Models;

/// <summary>
/// Payload for updating an existing ticket where status, priority, request type,
/// and custom fields may be specified by display name rather than by numeric ID.
/// All properties are optional — only non-null values are resolved and sent.
/// </summary>
/// <remarks>
/// Pass this model to <see cref="WhdClient.UpdateTicketAsync"/> which uses the
/// <see cref="Metadata.IWhdMetadataCache"/> to translate names to IDs before
/// sending the request. Call <see cref="WhdClient.InitializeMetadataCacheAsync"/>
/// at least once before updating tickets with named values.
/// Numeric ID strings (e.g. <c>"3"</c>) are also accepted wherever a name is expected.
/// </remarks>
public sealed class NamedUpdateTicketRequest
{
    /// <summary>Updated subject / title of the ticket.</summary>
    public string? Subject { get; init; }

    /// <summary>Updated detailed description.</summary>
    public string? Detail { get; init; }

    /// <summary>
    /// Status type identified by display name (e.g. <c>"Resolved"</c>)
    /// or numeric ID string (e.g. <c>"3"</c>).
    /// Leave <c>null</c> to leave the status unchanged.
    /// </summary>
    public string? StatusType { get; init; }

    /// <summary>
    /// Priority type identified by display name (e.g. <c>"Critical"</c>)
    /// or numeric ID string (e.g. <c>"1"</c>).
    /// Leave <c>null</c> to leave the priority unchanged.
    /// </summary>
    public string? PriorityType { get; init; }

    /// <summary>
    /// Request type / category identified by name (e.g. <c>"Hardware"</c>)
    /// or numeric ID string (e.g. <c>"5"</c>).
    /// Leave <c>null</c> to leave the request type unchanged.
    /// </summary>
    public string? RequestType { get; init; }

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
    /// Each entry identifies the field by label or numeric ID string;
    /// the metadata cache resolves labels to field definition IDs automatically.
    /// </summary>
    public List<NamedCustomFieldValue>? CustomFields { get; init; }
}
