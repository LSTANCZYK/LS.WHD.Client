using System.ComponentModel;
using LS.WHD.Client.Models;
using ModelContextProtocol.Server;

namespace LS.WHD.Client.Mcp.Tools;

/// <summary>MCP tools for asset (inventory) operations.</summary>
[McpServerToolType]
internal sealed class AssetTools
{
    private readonly WhdClientService _service;

    public AssetTools(WhdClientService service) => _service = service;

    [McpServerTool(Name = "list_assets")]
    [Description("Returns a paged list of assets (hardware / software inventory) from Web Help Desk.")]
    public async Task<IReadOnlyList<Asset>> ListAssetsAsync(
        [Description("Maximum number of assets to return (1–100, default 25).")] int? limit = null,
        [Description("Zero-based start index for pagination.")] int? start = null,
        CancellationToken cancellationToken = default)
    {
        var pagination = (limit.HasValue || start.HasValue)
            ? new PaginationOptions { Limit = limit ?? 25, Start = start ?? 0 }
            : null;

        return await _service.Client.Assets.ListAsync(pagination, cancellationToken);
    }

    [McpServerTool(Name = "search_assets")]
    [Description("Searches for assets whose name or asset tag contains the given term.")]
    public async Task<IReadOnlyList<Asset>> SearchAssetsAsync(
        [Description("Search term to match against asset name or tag.")] string search,
        [Description("Maximum number of results to return (1–100, default 25).")] int? limit = null,
        [Description("Zero-based start index for pagination.")] int? start = null,
        CancellationToken cancellationToken = default)
    {
        var pagination = (limit.HasValue || start.HasValue)
            ? new PaginationOptions { Limit = limit ?? 25, Start = start ?? 0 }
            : null;

        return await _service.Client.Assets.SearchAsync(search, pagination, cancellationToken);
    }
}
