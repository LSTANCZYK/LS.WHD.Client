using System.ComponentModel;
using LS.WHD.Client.Models;
using ModelContextProtocol.Server;

namespace LS.WHD.Client.Mcp.Tools;

/// <summary>MCP tools for ticket note operations.</summary>
[McpServerToolType]
internal sealed class NoteTools
{
    private readonly WhdClientService _service;

    public NoteTools(WhdClientService service) => _service = service;

    [McpServerTool(Name = "add_ticket_note")]
    [Description("Adds a note (comment / journal entry) to an existing ticket.")]
    public async Task<TicketNote?> AddTicketNoteAsync(
        [Description("The ID of the ticket to add the note to.")] long ticketId,
        [Description("Text content of the note.")] string noteText,
        [Description(
            "When true the note is visible to the client in the self-service portal. " +
            "Defaults to false (internal-only note).")] bool isClientNote = false,
        [Description("When true the note is marked as a solution note.")] bool isSolutionNote = false,
        [Description("Comma-separated list of additional email addresses to notify.")] string? emailsTo = null,
        CancellationToken cancellationToken = default)
    {
        var request = new CreateNoteRequest
        {
            TicketId       = ticketId,
            NoteText       = noteText,
            IsClientNote   = isClientNote,
            IsSolutionNote = isSolutionNote,
            EmailsTo       = emailsTo
        };

        return await _service.Client.Notes.CreateAsync(request, cancellationToken);
    }

    [McpServerTool(Name = "list_ticket_notes")]
    [Description("Returns all notes for a given ticket.")]
    public async Task<IReadOnlyList<TicketNote>> ListTicketNotesAsync(
        [Description("The ID of the ticket whose notes to retrieve.")] long ticketId,
        CancellationToken cancellationToken = default)
    {
        return await _service.Client.Notes.ListByTicketAsync(ticketId, cancellationToken);
    }
}
