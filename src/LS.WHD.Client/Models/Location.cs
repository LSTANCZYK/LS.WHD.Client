namespace LS.WHD.Client.Models;

/// <summary>
/// Represents a physical or logical location in Web Help Desk.
/// </summary>
public sealed class Location
{
    /// <summary>Internal unique identifier.</summary>
    public long Id { get; set; }

    /// <summary>Location name.</summary>
    public string? Name { get; set; }

    /// <summary>Street address.</summary>
    public string? Address1 { get; set; }

    /// <summary>Second address line.</summary>
    public string? Address2 { get; set; }

    /// <summary>City.</summary>
    public string? City { get; set; }

    /// <summary>State or province.</summary>
    public string? State { get; set; }

    /// <summary>ZIP / postal code.</summary>
    public string? Zip { get; set; }

    /// <summary>Country.</summary>
    public string? Country { get; set; }

    /// <summary>Phone number for this location.</summary>
    public string? Phone { get; set; }
}
