namespace LS.WHD.Client.Metadata;

/// <summary>
/// A cache of Web Help Desk lookup metadata that enables resolving display names
/// (e.g. "High Priority", "IT Support") to the numeric IDs required by the API.
/// </summary>
/// <remarks>
/// Call <see cref="InitializeAsync"/> (or <see cref="WhdClient.InitializeMetadataCacheAsync"/>)
/// once before using any name-based resolution method.
/// Use <see cref="RefreshAsync"/> to reload metadata after changes in WHD.
/// </remarks>
public interface IWhdMetadataCache
{
    /// <summary>
    /// Indicates whether the cache has been populated at least once.
    /// </summary>
    bool IsInitialized { get; }

    /// <summary>
    /// Loads all lookup metadata from WHD and populates the cache.
    /// Safe to call multiple times; subsequent calls refresh the cache.
    /// </summary>
    Task InitializeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Reloads all lookup metadata, replacing the current cache contents.
    /// Equivalent to calling <see cref="InitializeAsync"/> again.
    /// </summary>
    Task RefreshAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Resolves a priority type name or numeric ID string to its numeric ID.
    /// </summary>
    /// <param name="nameOrId">
    /// Display name (e.g. <c>"High"</c>) or numeric ID as a string (e.g. <c>"2"</c>).
    /// </param>
    /// <returns>The numeric priority type ID.</returns>
    /// <exception cref="Exceptions.WhdMetadataException">
    /// Thrown when <paramref name="nameOrId"/> is not a valid numeric ID and is either
    /// not found in the cache or matches multiple entries with the same name.
    /// </exception>
    long ResolvePriorityId(string nameOrId);

    /// <summary>
    /// Resolves a request type (category) name or numeric ID string to its numeric ID.
    /// </summary>
    /// <param name="nameOrId">
    /// Display name (e.g. <c>"IT Support"</c>) or numeric ID as a string (e.g. <c>"10"</c>).
    /// </param>
    long ResolveRequestTypeId(string nameOrId);

    /// <summary>
    /// Resolves a custom field label or numeric ID string to its numeric field definition ID.
    /// </summary>
    /// <param name="nameOrId">
    /// Field label (e.g. <c>"Department"</c>) or numeric ID as a string (e.g. <c>"3"</c>).
    /// </param>
    long ResolveCustomFieldId(string nameOrId);

    /// <summary>
    /// Resolves a status type name or numeric ID string to its numeric ID.
    /// </summary>
    /// <param name="nameOrId">
    /// Display name (e.g. <c>"Open"</c>) or numeric ID as a string (e.g. <c>"1"</c>).
    /// </param>
    long ResolveStatusTypeId(string nameOrId);
}
