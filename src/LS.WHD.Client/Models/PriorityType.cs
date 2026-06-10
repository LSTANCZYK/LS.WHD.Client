namespace LS.WHD.Client.Models;

/// <summary>
/// Represents a priority type (e.g. "Critical", "High", "Medium", "Low")
/// in Web Help Desk.
/// </summary>
public sealed class PriorityType
{
    /// <summary>Internal unique identifier.</summary>
    public long Id { get; set; }

    /// <summary>Display name of the priority.</summary>
    public string? Name { get; set; }
}
