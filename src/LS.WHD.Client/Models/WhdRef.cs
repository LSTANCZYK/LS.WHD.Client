namespace LS.WHD.Client.Models;

/// <summary>
/// A lightweight reference object used throughout the WHD API to represent
/// a related entity (e.g. a location, request type, client) inline.
/// </summary>
public sealed class WhdRef
{
    /// <summary>Unique identifier of the referenced entity.</summary>
    public long Id { get; set; }

    /// <summary>Human-readable name of the referenced entity.</summary>
    public string? Name { get; set; }
}
