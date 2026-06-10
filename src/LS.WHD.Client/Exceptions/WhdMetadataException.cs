namespace LS.WHD.Client.Exceptions;

/// <summary>
/// Thrown when a name-based metadata lookup fails because the requested
/// label was not found in the cache or is ambiguous (multiple entries share the same name).
/// </summary>
public sealed class WhdMetadataException : Exception
{
    /// <summary>The category of metadata that could not be resolved (e.g. "Priority", "RequestType").</summary>
    public string MetadataKind { get; }

    /// <summary>The name or value that could not be resolved.</summary>
    public string LookupValue { get; }

    /// <inheritdoc cref="WhdMetadataException"/>
    public WhdMetadataException(string metadataKind, string lookupValue, string message)
        : base(message)
    {
        MetadataKind = metadataKind;
        LookupValue  = lookupValue;
    }
}
