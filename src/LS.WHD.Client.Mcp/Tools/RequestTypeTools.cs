using System.ComponentModel;
using LS.WHD.Client.Models;
using ModelContextProtocol.Server;

namespace LS.WHD.Client.Mcp.Tools;

/// <summary>MCP tools for request type (ticket category) operations.</summary>
[McpServerToolType]
internal sealed class RequestTypeTools
{
    private readonly WhdClientService _service;

    public RequestTypeTools(WhdClientService service) => _service = service;

    [McpServerTool(Name = "list_request_types")]
    [Description("Returns all request types (ticket categories) configured in Web Help Desk.")]
    public async Task<IReadOnlyList<RequestType>> ListRequestTypesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _service.Client.RequestTypes.ListAsync(cancellationToken: cancellationToken);
    }
}
