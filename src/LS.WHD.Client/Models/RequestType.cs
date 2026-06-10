namespace LS.WHD.Client.Models;

/// <summary>
/// Represents a request type (category / subcategory) in Web Help Desk.
/// Request types form a hierarchy; a type without a <see cref="Parent"/>
/// is a top-level category.
/// </summary>
public sealed class RequestType
{
    /// <summary>Internal unique identifier.</summary>
    public long Id { get; set; }

    /// <summary>Name of the request type.</summary>
    public string? Name { get; set; }

    /// <summary>
    /// Parent request type. <c>null</c> for top-level categories.
    /// </summary>
    public WhdRef? Parent { get; set; }
}
