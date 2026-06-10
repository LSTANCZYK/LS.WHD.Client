using LS.WHD.Client.Http;
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

    /// <inheritdoc/>
    public void Dispose() => _http.Dispose();
}
