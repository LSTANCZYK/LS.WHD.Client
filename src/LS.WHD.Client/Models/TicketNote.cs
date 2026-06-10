namespace LS.WHD.Client.Models;

/// <summary>
/// Represents a note (journal entry / comment) on a ticket in Web Help Desk.
/// </summary>
public sealed class TicketNote
{
    /// <summary>Internal unique identifier of the note.</summary>
    public long Id { get; set; }

    /// <summary>Text content of the note.</summary>
    public string? NoteText { get; set; }

    /// <summary>Whether this note is visible to the client (false = internal only).</summary>
    public bool? IsClientNote { get; set; }

    /// <summary>Whether the note is a solution note.</summary>
    public bool? IsSolutionNote { get; set; }

    /// <summary>Author of the note (reference to a tech or client).</summary>
    public WhdRef? Author { get; set; }

    /// <summary>Date and time the note was created.</summary>
    public DateTimeOffset? CreatedDate { get; set; }

    /// <summary>Email addresses notified when this note was added.</summary>
    public string? EmailsTo { get; set; }
}
