using System.ComponentModel;
using LS.WHD.Client.Models;
using ModelContextProtocol.Server;

namespace LS.WHD.Client.Mcp.Tools;

/// <summary>MCP tools for WHD lookup data (priorities, statuses, technicians).</summary>
[McpServerToolType]
internal sealed class LookupTools
{
    private readonly WhdClientService _service;

    public LookupTools(WhdClientService service) => _service = service;

    [McpServerTool(Name = "list_priorities")]
    [Description("Returns all priority types configured in Web Help Desk (e.g. Critical, High, Medium, Low).")]
    public async Task<IReadOnlyList<PriorityType>> ListPrioritiesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _service.Client.Lookups.GetPriorityTypesAsync(cancellationToken);
    }

    [McpServerTool(Name = "list_status_types")]
    [Description("Returns all status types configured in Web Help Desk (e.g. Open, In Progress, Resolved, Closed).")]
    public async Task<IReadOnlyList<StatusType>> ListStatusTypesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _service.Client.Lookups.GetStatusTypesAsync(cancellationToken);
    }

    [McpServerTool(Name = "list_technicians")]
    [Description("Returns a paged list of technician accounts in Web Help Desk.")]
    public async Task<IReadOnlyList<Tech>> ListTechniciansAsync(
        [Description("Maximum number of technicians to return (1–100, default 25).")] int? limit = null,
        [Description("Zero-based start index for pagination.")] int? start = null,
        CancellationToken cancellationToken = default)
    {
        var pagination = (limit.HasValue || start.HasValue)
            ? new PaginationOptions { Limit = limit ?? 25, Start = start ?? 0 }
            : null;

        return await _service.Client.Lookups.GetTechsAsync(pagination, cancellationToken);
    }
}
