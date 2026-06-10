using System.ComponentModel;
using LS.WHD.Client.Models;
using ModelContextProtocol.Server;

namespace LS.WHD.Client.Mcp.Tools;

/// <summary>MCP tools for client (end-user) operations.</summary>
[McpServerToolType]
internal sealed class ClientTools
{
    private readonly WhdClientService _service;

    public ClientTools(WhdClientService service) => _service = service;

    [McpServerTool(Name = "list_clients")]
    [Description("Returns a paged list of client (end-user) accounts from Web Help Desk.")]
    public async Task<IReadOnlyList<HelpDeskClient>> ListClientsAsync(
        [Description("Maximum number of clients to return (1–100, default 25).")] int? limit = null,
        [Description("Zero-based start index for pagination.")] int? start = null,
        CancellationToken cancellationToken = default)
    {
        var pagination = (limit.HasValue || start.HasValue)
            ? new PaginationOptions { Limit = limit ?? 25, Start = start ?? 0 }
            : null;

        return await _service.Client.Clients.ListAsync(pagination, cancellationToken);
    }

    [McpServerTool(Name = "search_clients")]
    [Description("Searches for client accounts whose username or email contains the given term.")]
    public async Task<IReadOnlyList<HelpDeskClient>> SearchClientsAsync(
        [Description("Search term to match against username or email.")] string search,
        [Description("Maximum number of results to return (1–100, default 25).")] int? limit = null,
        [Description("Zero-based start index for pagination.")] int? start = null,
        CancellationToken cancellationToken = default)
    {
        var pagination = (limit.HasValue || start.HasValue)
            ? new PaginationOptions { Limit = limit ?? 25, Start = start ?? 0 }
            : null;

        return await _service.Client.Clients.SearchAsync(search, pagination, cancellationToken);
    }
}
