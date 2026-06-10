using LS.WHD.Client.Http;
using LS.WHD.Client.Models;

namespace LS.WHD.Client.Services;

/// <summary>
/// Shared helpers used by all service implementations.
/// </summary>
internal static class ServiceHelpers
{
    /// <summary>
    /// Builds a <see cref="QueryParameters"/> instance from <paramref name="pagination"/>,
    /// or returns <c>null</c> if pagination is <c>null</c>.
    /// </summary>
    public static QueryParameters? BuildPagination(PaginationOptions? pagination)
    {
        if (pagination is null) return null;
        return new QueryParameters(
            ("limit", pagination.Limit.ToString()),
            ("start", pagination.Start.ToString()));
    }
}
