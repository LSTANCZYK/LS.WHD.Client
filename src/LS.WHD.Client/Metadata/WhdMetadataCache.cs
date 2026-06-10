using LS.WHD.Client.Exceptions;
using LS.WHD.Client.Services;

namespace LS.WHD.Client.Metadata;

/// <summary>
/// Default implementation of <see cref="IWhdMetadataCache"/> that fetches lookup
/// data from the WHD API and stores it in case-insensitive in-memory dictionaries.
/// </summary>
internal sealed class WhdMetadataCache : IWhdMetadataCache
{
    private readonly ILookupService      _lookups;
    private readonly IRequestTypeService _requestTypes;
    private readonly ICustomFieldService _customFields;

    // Maps lowercase name → id (only non-ambiguous entries are stored here)
    private Dictionary<string, long> _priorityIndex    = new(StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, long> _requestTypeIndex = new(StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, long> _customFieldIndex = new(StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, long> _statusTypeIndex  = new(StringComparer.OrdinalIgnoreCase);

    // Tracks names that appear more than once so a clear error can be raised on resolution
    private HashSet<string> _ambiguousPriorities    = new(StringComparer.OrdinalIgnoreCase);
    private HashSet<string> _ambiguousRequestTypes  = new(StringComparer.OrdinalIgnoreCase);
    private HashSet<string> _ambiguousCustomFields  = new(StringComparer.OrdinalIgnoreCase);
    private HashSet<string> _ambiguousStatusTypes   = new(StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc/>
    public bool IsInitialized { get; private set; }

    internal WhdMetadataCache(
        ILookupService      lookups,
        IRequestTypeService requestTypes,
        ICustomFieldService customFields)
    {
        _lookups      = lookups;
        _requestTypes = requestTypes;
        _customFields = customFields;
    }

    /// <inheritdoc/>
    public Task InitializeAsync(CancellationToken cancellationToken = default)
        => RefreshAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        var priorityTask     = _lookups.GetPriorityTypesAsync(cancellationToken);
        var statusTask       = _lookups.GetStatusTypesAsync(cancellationToken);
        var requestTypeTask  = _requestTypes.ListAsync(cancellationToken: cancellationToken);
        var customFieldTask  = _customFields.ListAsync(cancellationToken);

        await Task.WhenAll(priorityTask, statusTask, requestTypeTask, customFieldTask)
                  .ConfigureAwait(false);

        (_priorityIndex,   _ambiguousPriorities)   = BuildIndex(
            priorityTask.Result.Select(p => (p.Name, p.Id)));

        (_statusTypeIndex, _ambiguousStatusTypes)   = BuildIndex(
            statusTask.Result.Select(s => (s.Name, s.Id)));

        (_requestTypeIndex, _ambiguousRequestTypes) = BuildIndex(
            requestTypeTask.Result.Select(r => (r.Name, r.Id)));

        (_customFieldIndex, _ambiguousCustomFields) = BuildIndex(
            customFieldTask.Result.Select(f => (f.FieldLabel, f.Id)));

        IsInitialized = true;
    }

    /// <inheritdoc/>
    public long ResolvePriorityId(string nameOrId)
        => Resolve(nameOrId, "Priority", _priorityIndex, _ambiguousPriorities);

    /// <inheritdoc/>
    public long ResolveRequestTypeId(string nameOrId)
        => Resolve(nameOrId, "RequestType", _requestTypeIndex, _ambiguousRequestTypes);

    /// <inheritdoc/>
    public long ResolveCustomFieldId(string nameOrId)
        => Resolve(nameOrId, "CustomField", _customFieldIndex, _ambiguousCustomFields);

    /// <inheritdoc/>
    public long ResolveStatusTypeId(string nameOrId)
        => Resolve(nameOrId, "StatusType", _statusTypeIndex, _ambiguousStatusTypes);

    // ──────────────────────── helpers ────────────────────────

    /// <summary>
    /// Resolves <paramref name="nameOrId"/> to a numeric ID.
    /// If the value looks like a number it is parsed directly (no cache lookup needed).
    /// Otherwise the cache is searched; missing or ambiguous names throw <see cref="WhdMetadataException"/>.
    /// </summary>
    private long Resolve(
        string nameOrId,
        string kind,
        Dictionary<string, long> index,
        HashSet<string> ambiguous)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nameOrId);

        // Fast path: caller already has a numeric ID
        if (long.TryParse(nameOrId, out var numericId))
            return numericId;

        EnsureInitialized();

        if (ambiguous.Contains(nameOrId))
            throw new WhdMetadataException(
                kind, nameOrId,
                $"{kind} name '{nameOrId}' is ambiguous: multiple entries share this name. " +
                "Use the numeric ID instead.");

        if (index.TryGetValue(nameOrId, out var resolvedId))
            return resolvedId;

        throw new WhdMetadataException(
            kind, nameOrId,
            $"{kind} '{nameOrId}' was not found in the metadata cache. " +
            "Check the spelling or call RefreshAsync() if the data was recently added.");
    }

    private void EnsureInitialized()
    {
        if (!IsInitialized)
            throw new InvalidOperationException(
                "The metadata cache has not been initialized. " +
                "Call WhdClient.InitializeMetadataCacheAsync() (or Metadata.InitializeAsync()) " +
                "before using name-based resolution.");
    }

    /// <summary>
    /// Builds a name→id dictionary from a sequence of (name?, id) pairs.
    /// Names that appear more than once are removed from the dictionary and
    /// tracked in the returned ambiguity set so they can produce a clear error.
    /// </summary>
    private static (Dictionary<string, long> index, HashSet<string> ambiguous) BuildIndex(
        IEnumerable<(string? name, long id)> items)
    {
        var index     = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);
        var ambiguous = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var (name, id) in items)
        {
            if (string.IsNullOrWhiteSpace(name))
                continue;

            if (index.ContainsKey(name))
            {
                // Mark as ambiguous and remove from the lookup index
                ambiguous.Add(name);
                index.Remove(name);
            }
            else if (!ambiguous.Contains(name))
            {
                index[name] = id;
            }
        }

        return (index, ambiguous);
    }
}
