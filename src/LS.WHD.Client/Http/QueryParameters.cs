namespace LS.WHD.Client.Http;

/// <summary>
/// Represents a set of additional query-string parameters to append to an API request.
/// </summary>
public sealed class QueryParameters : Dictionary<string, string>
{
    /// <summary>Creates an empty instance.</summary>
    public QueryParameters() { }

    /// <summary>
    /// Creates a pre-populated instance with the given key/value pairs.
    /// </summary>
    public QueryParameters(params (string key, string value)[] pairs)
    {
        foreach (var (key, value) in pairs)
            this[key] = value;
    }
}
