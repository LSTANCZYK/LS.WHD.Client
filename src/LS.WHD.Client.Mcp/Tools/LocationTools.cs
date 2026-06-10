using System.ComponentModel;
using LS.WHD.Client.Models;
using ModelContextProtocol.Server;

namespace LS.WHD.Client.Mcp.Tools;

/// <summary>MCP tools for location operations.</summary>
[McpServerToolType]
internal sealed class LocationTools
{
    private readonly WhdClientService _service;

    public LocationTools(WhdClientService service) => _service = service;

    [McpServerTool(Name = "list_locations")]
    [Description("Returns all locations configured in Web Help Desk.")]
    public async Task<IReadOnlyList<Location>> ListLocationsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _service.Client.Locations.ListAsync(cancellationToken: cancellationToken);
    }
}
