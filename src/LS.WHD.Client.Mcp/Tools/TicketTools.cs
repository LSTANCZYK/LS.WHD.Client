using System.ComponentModel;
using LS.WHD.Client.Models;
using ModelContextProtocol.Server;

namespace LS.WHD.Client.Mcp.Tools;

/// <summary>MCP tools for ticket (request) operations.</summary>
[McpServerToolType]
internal sealed class TicketTools
{
    private readonly WhdClientService _service;

    public TicketTools(WhdClientService service) => _service = service;

    [McpServerTool(Name = "list_tickets")]
    [Description("Returns a paged list of tickets from Web Help Desk.")]
    public async Task<IReadOnlyList<Ticket>> ListTicketsAsync(
        [Description("Maximum number of tickets to return (1–100, default 25).")] int? limit = null,
        [Description("Zero-based start index for pagination.")] int? start = null,
        CancellationToken cancellationToken = default)
    {
        var pagination = (limit.HasValue || start.HasValue)
            ? new PaginationOptions { Limit = limit ?? 25, Start = start ?? 0 }
            : null;

        return await _service.Client.Tickets.ListAsync(pagination, cancellationToken);
    }

    [McpServerTool(Name = "get_ticket")]
    [Description("Returns the ticket with the specified ID.")]
    public async Task<Ticket?> GetTicketAsync(
        [Description("The numeric ID of the ticket to retrieve.")] long id,
        CancellationToken cancellationToken = default)
    {
        return await _service.Client.Tickets.GetAsync(id, cancellationToken);
    }

    [McpServerTool(Name = "create_ticket")]
    [Description(
        "Creates a new ticket in Web Help Desk. " +
        "Priority and request type may be specified by display name (e.g. \"High\", \"IT Support\") " +
        "or by numeric ID. The metadata cache is initialised automatically on first use.")]
    public async Task<Ticket?> CreateTicketAsync(
        [Description("Short description / subject of the ticket (required).")] string subject,
        [Description("Detailed description of the issue.")] string? detail = null,
        [Description("ID of the client (end user) for whom the ticket is created.")] long? clientId = null,
        [Description("Priority type by display name (e.g. \"High\") or numeric ID.")] string? priorityType = null,
        [Description("Request type / category by name (e.g. \"IT Support\") or numeric ID.")] string? requestType = null,
        [Description("ID of the location to associate with the ticket.")] long? locationId = null,
        [Description("ID of the asset to link to the ticket.")] long? assetId = null,
        [Description("ID of the technician to assign to the ticket.")] long? techId = null,
        [Description("Due date for resolving the ticket (ISO-8601).")] DateTimeOffset? dueDate = null,
        CancellationToken cancellationToken = default)
    {
        await _service.EnsureMetadataAsync(cancellationToken);

        var request = new NamedCreateTicketRequest
        {
            Subject      = subject,
            Detail       = detail,
            ClientId     = clientId,
            PriorityType = priorityType,
            RequestType  = requestType,
            LocationId   = locationId,
            AssetId      = assetId,
            TechId       = techId,
            DueDate      = dueDate
        };

        return await _service.Client.CreateTicketAsync(request, cancellationToken);
    }

    [McpServerTool(Name = "update_ticket")]
    [Description(
        "Updates an existing ticket. Only the fields you provide are changed. " +
        "Status, priority, and request type may be specified by display name or numeric ID.")]
    public async Task<Ticket?> UpdateTicketAsync(
        [Description("The numeric ID of the ticket to update.")] long id,
        [Description("New subject / title.")] string? subject = null,
        [Description("New detailed description.")] string? detail = null,
        [Description("New status by display name (e.g. \"Resolved\") or numeric ID.")] string? statusType = null,
        [Description("New priority by display name (e.g. \"Critical\") or numeric ID.")] string? priorityType = null,
        [Description("New request type by display name or numeric ID.")] string? requestType = null,
        [Description("ID of the new assigned technician.")] long? techId = null,
        [Description("ID of the new location.")] long? locationId = null,
        [Description("ID of the new linked asset.")] long? assetId = null,
        [Description("Updated due date (ISO-8601).")] DateTimeOffset? dueDate = null,
        CancellationToken cancellationToken = default)
    {
        await _service.EnsureMetadataAsync(cancellationToken);

        var request = new NamedUpdateTicketRequest
        {
            Subject      = subject,
            Detail       = detail,
            StatusType   = statusType,
            PriorityType = priorityType,
            RequestType  = requestType,
            TechId       = techId,
            LocationId   = locationId,
            AssetId      = assetId,
            DueDate      = dueDate
        };

        return await _service.Client.UpdateTicketAsync(id, request, cancellationToken);
    }
}
