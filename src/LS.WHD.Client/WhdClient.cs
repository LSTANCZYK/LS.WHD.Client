using LS.WHD.Client.Http;
using LS.WHD.Client.Metadata;
using LS.WHD.Client.Models;
using LS.WHD.Client.Services;

namespace LS.WHD.Client;

/// <summary>
/// Top-level client for the SolarWinds Web Help Desk REST API.
/// </summary>
/// <remarks>
/// Instantiate once and reuse it across your application for best performance.
/// Dispose the client when it is no longer needed.
/// </remarks>
/// <example>
/// <code>
/// var client = new WhdClient(new WhdClientOptions
/// {
///     BaseUrl = "https://helpdesk.example.com:8081",
///     ApiKey  = "your-api-key"
/// });
///
/// var tickets = await client.Tickets.ListAsync();
/// </code>
/// </example>
public sealed class WhdClient : IDisposable
{
    private readonly WhdHttpClient _http;

    /// <summary>
    /// Initializes a new instance of <see cref="WhdClient"/> with the given options.
    /// </summary>
    /// <param name="options">Connection and authentication options.</param>
    public WhdClient(WhdClientOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _http = new WhdHttpClient(options);

        Tickets      = new TicketService(_http);
        Clients      = new ClientService(_http);
        Assets       = new AssetService(_http);
        Locations    = new LocationService(_http);
        RequestTypes = new RequestTypeService(_http);
        Notes        = new NoteService(_http);
        Lookups      = new LookupService(_http);
        CustomFields = new CustomFieldService(_http);
        Metadata     = new WhdMetadataCache(Lookups, RequestTypes, CustomFields);
    }

    // ──────────────────────── Service accessors ────────────────────────

    /// <summary>Operations on tickets (requests).</summary>
    public ITicketService Tickets { get; }

    /// <summary>Operations on client (end-user) accounts.</summary>
    public IClientService Clients { get; }

    /// <summary>Operations on assets (hardware / software inventory).</summary>
    public IAssetService Assets { get; }

    /// <summary>Operations on locations.</summary>
    public ILocationService Locations { get; }

    /// <summary>Operations on request types (ticket categories).</summary>
    public IRequestTypeService RequestTypes { get; }

    /// <summary>Operations on ticket notes / journal entries.</summary>
    public INoteService Notes { get; }

    /// <summary>Lookup operations (status types, priority types, technicians).</summary>
    public ILookupService Lookups { get; }

    /// <summary>Operations on custom field definitions.</summary>
    public ICustomFieldService CustomFields { get; }

    /// <summary>
    /// In-memory cache of WHD lookup metadata (priorities, request types, custom fields,
    /// status types) that powers name-based resolution.
    /// Call <see cref="InitializeMetadataCacheAsync"/> before using
    /// <see cref="CreateTicketAsync"/> or <see cref="UpdateTicketAsync"/>.
    /// </summary>
    public IWhdMetadataCache Metadata { get; }

    // ──────────────────────── Metadata helpers ────────────────────────

    /// <summary>
    /// Loads all WHD lookup metadata (priorities, request types, custom fields,
    /// status types) into the in-memory <see cref="Metadata"/> cache.
    /// </summary>
    /// <remarks>
    /// Call this once after creating the client. Re-call (or call <see cref="IWhdMetadataCache.RefreshAsync"/>)
    /// whenever lookup data changes in WHD and you need the cache to reflect the latest values.
    /// </remarks>
    public Task InitializeMetadataCacheAsync(CancellationToken cancellationToken = default)
        => Metadata.InitializeAsync(cancellationToken);

    // ──────────────────────── Name-based ticket helpers ────────────────────────

    /// <summary>
    /// Creates a new ticket, resolving priority, request type, and custom field names
    /// to their numeric IDs using the <see cref="Metadata"/> cache.
    /// </summary>
    /// <param name="request">
    /// Ticket creation payload with optional name-based references.
    /// Numeric ID strings are also accepted wherever a display name is expected.
    /// </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created ticket as returned by WHD.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the metadata cache has not been initialized.
    /// </exception>
    /// <exception cref="Exceptions.WhdMetadataException">
    /// Thrown when a provided name cannot be resolved (not found or ambiguous).
    /// </exception>
    public Task<Ticket?> CreateTicketAsync(
        NamedCreateTicketRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var resolved = new CreateTicketRequest
        {
            Subject        = request.Subject,
            Detail         = request.Detail,
            ClientId       = request.ClientId,
            PriorityTypeId = request.PriorityType is not null
                                 ? Metadata.ResolvePriorityId(request.PriorityType)
                                 : null,
            RequestTypeId  = request.RequestType is not null
                                 ? Metadata.ResolveRequestTypeId(request.RequestType)
                                 : null,
            LocationId     = request.LocationId,
            AssetId        = request.AssetId,
            TechId         = request.TechId,
            DueDate        = request.DueDate,
            CustomFields   = ResolveCustomFields(request.CustomFields)
        };

        return Tickets.CreateAsync(resolved, cancellationToken);
    }

    /// <summary>
    /// Updates an existing ticket, resolving status, priority, request type,
    /// and custom field names to their numeric IDs using the <see cref="Metadata"/> cache.
    /// </summary>
    /// <param name="id">The ID of the ticket to update.</param>
    /// <param name="request">
    /// Ticket update payload with optional name-based references.
    /// Only non-null properties are sent. Numeric ID strings are also accepted
    /// wherever a display name is expected.
    /// </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated ticket as returned by WHD.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the metadata cache has not been initialized.
    /// </exception>
    /// <exception cref="Exceptions.WhdMetadataException">
    /// Thrown when a provided name cannot be resolved (not found or ambiguous).
    /// </exception>
    public Task<Ticket?> UpdateTicketAsync(
        long id,
        NamedUpdateTicketRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var resolved = new UpdateTicketRequest
        {
            Subject        = request.Subject,
            Detail         = request.Detail,
            StatusTypeId   = request.StatusType is not null
                                 ? Metadata.ResolveStatusTypeId(request.StatusType)
                                 : null,
            PriorityTypeId = request.PriorityType is not null
                                 ? Metadata.ResolvePriorityId(request.PriorityType)
                                 : null,
            RequestTypeId  = request.RequestType is not null
                                 ? Metadata.ResolveRequestTypeId(request.RequestType)
                                 : null,
            TechId         = request.TechId,
            LocationId     = request.LocationId,
            AssetId        = request.AssetId,
            DueDate        = request.DueDate,
            CustomFields   = ResolveCustomFields(request.CustomFields)
        };

        return Tickets.UpdateAsync(id, resolved, cancellationToken);
    }

    /// <inheritdoc/>
    public void Dispose() => _http.Dispose();

    // ──────────────────────── private helpers ────────────────────────

    private List<CustomField>? ResolveCustomFields(List<NamedCustomFieldValue>? fields)
    {
        if (fields is null)
            return null;

        return fields.Select(f => new CustomField
        {
            Id         = Metadata.ResolveCustomFieldId(f.Field),
            RestValue  = f.Value
        }).ToList();
    }
}
