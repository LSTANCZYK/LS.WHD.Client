namespace LS.WHD.Client.Models;

/// <summary>
/// Represents pagination settings for list operations.
/// </summary>
public sealed class PaginationOptions
{
    /// <summary>
    /// Number of records to return per page.
    /// The WHD API default is typically 25; maximum is 100.
    /// </summary>
    public int Limit { get; init; } = 25;

    /// <summary>
    /// Zero-based offset of the first record to return.
    /// Use <c>pageNumber * Limit</c> to implement page-based navigation.
    /// </summary>
    public int Start { get; init; } = 0;
}
