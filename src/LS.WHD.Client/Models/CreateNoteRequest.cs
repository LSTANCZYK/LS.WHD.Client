namespace LS.WHD.Client.Models;

/// <summary>
/// Payload for adding a note to a ticket via <c>POST /TicketNotes</c>.
/// </summary>
public sealed class CreateNoteRequest
{
    /// <summary>The ID of the ticket this note belongs to.</summary>
    public required long TicketId { get; init; }

    /// <summary>Text content of the note (required).</summary>
    public required string NoteText { get; init; }

    /// <summary>
    /// Whether the note is visible to the client.
    /// Defaults to <c>false</c> (internal note).
    /// </summary>
    public bool IsClientNote { get; init; } = false;

    /// <summary>Whether the note represents a solution.</summary>
    public bool IsSolutionNote { get; init; } = false;

    /// <summary>
    /// Comma-separated list of additional email addresses to notify.
    /// </summary>
    public string? EmailsTo { get; init; }
}
