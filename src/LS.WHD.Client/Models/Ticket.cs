using System.Text.Json.Serialization;

namespace LS.WHD.Client.Models;

/// <summary>
/// Represents a WHD ticket (also referred to as a <em>request</em> in the product UI).
/// </summary>
public sealed class Ticket
{
    /// <summary>Internal unique identifier of the ticket.</summary>
    public long Id { get; set; }

    /// <summary>Human-readable ticket number displayed in the UI (e.g. "12345").</summary>
    [JsonPropertyName("problemtype")]
    public string? ProblemType { get; set; }

    /// <summary>Short description / subject of the ticket.</summary>
    public string? Subject { get; set; }

    /// <summary>Detailed description of the issue.</summary>
    public string? Detail { get; set; }

    /// <summary>Current status of the ticket.</summary>
    public WhdRef? StatusType { get; set; }

    /// <summary>Priority of the ticket.</summary>
    public WhdRef? PriorityType { get; set; }

    /// <summary>Category / request type of the ticket.</summary>
    public WhdRef? RequestType { get; set; }

    /// <summary>The client (end user) who submitted the ticket.</summary>
    public WhdRef? Client { get; set; }

    /// <summary>The technician assigned to the ticket.</summary>
    public WhdRef? Tech { get; set; }

    /// <summary>The location associated with the ticket.</summary>
    public WhdRef? Location { get; set; }

    /// <summary>The asset linked to the ticket.</summary>
    public WhdRef? Asset { get; set; }

    /// <summary>Date and time when the ticket was created.</summary>
    public DateTimeOffset? CreatedDate { get; set; }

    /// <summary>Date and time when the ticket was last updated.</summary>
    public DateTimeOffset? LastUpdated { get; set; }

    /// <summary>Due date for resolving the ticket.</summary>
    public DateTimeOffset? DueDate { get; set; }

    /// <summary>Custom field values attached to the ticket.</summary>
    public List<CustomField>? CustomFields { get; set; }
}
