namespace LS.WHD.Client.Models;

/// <summary>
/// Represents a status type (e.g. "Open", "Closed", "Resolved") in Web Help Desk.
/// </summary>
public sealed class StatusType
{
    /// <summary>Internal unique identifier.</summary>
    public long Id { get; set; }

    /// <summary>Display name of the status.</summary>
    public string? Name { get; set; }
}
