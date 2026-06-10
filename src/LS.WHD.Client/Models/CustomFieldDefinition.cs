namespace LS.WHD.Client.Models;

/// <summary>
/// Describes a custom field definition as returned by <c>GET /customfields</c>.
/// </summary>
public sealed class CustomFieldDefinition
{
    /// <summary>Internal unique identifier of the custom field definition.</summary>
    public long Id { get; set; }

    /// <summary>Display label shown in the UI.</summary>
    public string? FieldLabel { get; set; }

    /// <summary>
    /// Data type of the field (e.g. <c>TEXT</c>, <c>SELECT</c>, <c>DATE</c>, <c>NUMBER</c>).
    /// </summary>
    public string? FieldType { get; set; }

    /// <summary>Whether a value is required when creating or updating a ticket.</summary>
    public bool Required { get; set; }

    /// <summary>Display order of the field.</summary>
    public int OrderNumber { get; set; }
}
