namespace LS.WHD.Client.Models;

/// <summary>
/// Represents the value of a custom field on a ticket as returned by the WHD API.
/// </summary>
public sealed class CustomField
{
    /// <summary>The ID of the custom field definition.</summary>
    public long Id { get; set; }

    /// <summary>Display label for the field.</summary>
    public string? FieldLabel { get; set; }

    /// <summary>The current value of the field (display/string form).</summary>
    public string? Value { get; set; }

    /// <summary>
    /// The REST-serializable value of the field.
    /// For most field types this equals <see cref="Value"/>;
    /// for select fields it contains the option key.
    /// </summary>
    public string? RestValue { get; set; }

    /// <summary>Whether the field is required.</summary>
    public bool Required { get; set; }
}
