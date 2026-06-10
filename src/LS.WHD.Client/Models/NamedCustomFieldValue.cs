namespace LS.WHD.Client.Models;

/// <summary>
/// Represents a custom field value referenced by display label or numeric ID string,
/// used with <see cref="NamedCreateTicketRequest"/> and <see cref="NamedUpdateTicketRequest"/>.
/// </summary>
public sealed class NamedCustomFieldValue
{
    /// <summary>
    /// The custom field to set, identified by its display label (e.g. <c>"Department"</c>)
    /// or by its numeric ID as a string (e.g. <c>"3"</c>).
    /// The metadata cache resolves labels to IDs automatically.
    /// </summary>
    public required string Field { get; init; }

    /// <summary>The value to write to the field.</summary>
    public string? Value { get; init; }
}
