# LS.WHD.Client

A modern, idiomatic C# client library for the **SolarWinds Web Help Desk (WHD) REST API**.

- Targets **.NET 8**
- Uses `HttpClient` and `async`/`await` throughout
- Supports **API Key** and **Basic Auth** authentication
- Covers tickets, clients, assets, locations, request types, notes, and lookup data
- Handles pagination and error responses
- **Name-based metadata resolution** — refer to priorities, request types, and custom fields by display name

---

## Installation

```shell
dotnet add package LS.WHD.Client
```

> **Note:** The package is not yet published to NuGet. Clone and reference the project directly for now.

---

## Quick Start

```csharp
using LS.WHD.Client;

// 1. Configure the client
var client = new WhdClient(new WhdClientOptions
{
    BaseUrl = "https://helpdesk.example.com:8081",
    AuthMode = LS.WHD.Client.Authentication.AuthMode.ApiKey,
    ApiKey  = "your-api-key-here"
});

// 2. List the first 25 open tickets
var tickets = await client.Tickets.ListAsync();
foreach (var t in tickets)
    Console.WriteLine($"{t.Id}: {t.Subject}");

// 3. Clean up
client.Dispose();
```

---

## Authentication

SolarWinds Web Help Desk supports two authentication methods.

### API Key (recommended for server-to-server integrations)

Generate an API key in **WHD → Admin → Accounts → API Key** for a technician account.

```csharp
var options = new WhdClientOptions
{
    BaseUrl = "https://helpdesk.example.com:8081",
    AuthMode = LS.WHD.Client.Authentication.AuthMode.ApiKey,
    ApiKey   = "YOUR_API_KEY"
};
```

The key is appended as a query parameter (`?apiKey=…`) to every request, which is the convention documented in the WHD REST API guide.

### HTTP Basic Auth

```csharp
var options = new WhdClientOptions
{
    BaseUrl  = "https://helpdesk.example.com:8081",
    AuthMode = LS.WHD.Client.Authentication.AuthMode.BasicAuth,
    Username = "admin",
    Password = "p@ssword"
};
```

---

## Configuration Reference

| Property | Type | Default | Description |
|---|---|---|---|
| `BaseUrl` | `string` | — (required) | Base URL of the WHD server (no trailing slash) |
| `AuthMode` | `AuthMode` | `ApiKey` | Authentication method |
| `ApiKey` | `string?` | `null` | API key (used with `AuthMode.ApiKey`) |
| `Username` | `string?` | `null` | Username (used with `AuthMode.BasicAuth`) |
| `Password` | `string?` | `null` | Password (used with `AuthMode.BasicAuth`) |
| `IgnoreSslErrors` | `bool` | `false` | Skip TLS certificate validation (dev only) |
| `Timeout` | `TimeSpan` | 30 s | Per-request HTTP timeout |

---

## Name-Based Metadata Resolution

WHD REST APIs require numeric IDs for priorities, request types, and custom fields.
This library lets you use **display names** instead by loading lookup metadata into an
in-memory cache at startup.

### 1. Initialize the metadata cache

Call `InitializeMetadataCacheAsync()` once after creating the client:

```csharp
var client = new WhdClient(options);
await client.InitializeMetadataCacheAsync();
```

### 2. Create / update tickets by name

Use `NamedCreateTicketRequest` and `NamedUpdateTicketRequest` with `client.CreateTicketAsync`
and `client.UpdateTicketAsync`:

```csharp
// Create a ticket — use display names for priority, request type, and custom fields
var ticket = await client.CreateTicketAsync(new NamedCreateTicketRequest
{
    Subject      = "Printer is broken",
    Detail       = "Office printer on floor 2 prints blank pages.",
    ClientId     = 101,
    PriorityType = "High",          // resolved to its numeric ID automatically
    RequestType  = "IT Support",    // resolved to its numeric ID automatically
    CustomFields =
    [
        new NamedCustomFieldValue { Field = "Department", Value = "Finance" },
        new NamedCustomFieldValue { Field = "Cost Center", Value = "CC-200" }
    ]
});

// Update a ticket — again use names; only non-null fields are sent
await client.UpdateTicketAsync(ticket!.Id, new NamedUpdateTicketRequest
{
    StatusType   = "Resolved",
    PriorityType = "Low"
});
```

Numeric ID strings are also accepted, so existing workflows that pass IDs still work:

```csharp
// "2" is parsed as the numeric ID 2 — no cache lookup needed
await client.CreateTicketAsync(new NamedCreateTicketRequest
{
    Subject      = "Quick fix",
    PriorityType = "2"              // same as PriorityTypeId = 2 in the numeric API
});
```

### 3. Resolve names manually (optional)

You can call the resolver directly if you need the numeric ID for another purpose:

```csharp
long priorityId    = client.Metadata.ResolvePriorityId("High");
long requestTypeId = client.Metadata.ResolveRequestTypeId("IT Support");
long fieldId       = client.Metadata.ResolveCustomFieldId("Department");
long statusId      = client.Metadata.ResolveStatusTypeId("Resolved");
```

### 4. Refresh the cache

Call `RefreshAsync()` whenever metadata changes in WHD:

```csharp
await client.Metadata.RefreshAsync();
// or equivalently:
await client.InitializeMetadataCacheAsync();
```

### Error handling for name resolution

| Situation | Exception |
|---|---|
| Cache not yet initialized | `InvalidOperationException` |
| Name not found in cache | `WhdMetadataException` |
| Name matches multiple entries | `WhdMetadataException` (ambiguous) |

```csharp
using LS.WHD.Client.Exceptions;

try
{
    await client.CreateTicketAsync(new NamedCreateTicketRequest
    {
        Subject      = "Test",
        PriorityType = "Typo Priority"   // does not exist
    });
}
catch (WhdMetadataException ex)
{
    Console.WriteLine($"Could not resolve {ex.MetadataKind} '{ex.LookupValue}': {ex.Message}");
}
```

If two entries share the same name in WHD, resolution by name throws an ambiguous error.
In that case pass the numeric ID string directly (e.g. `"2"`) as a workaround.

---

## Supported Operations

### Tickets

```csharp
// List tickets (paged)
var tickets = await client.Tickets.ListAsync(
    new PaginationOptions { Limit = 50, Start = 0 });

// Get single ticket
var ticket = await client.Tickets.GetAsync(1234);

// Create ticket (numeric IDs — original API)
var created = await client.Tickets.CreateAsync(new CreateTicketRequest
{
    Subject       = "Printer is broken",
    Detail        = "Office printer on floor 2 prints blank pages.",
    ClientId      = 101,
    RequestTypeId = 10,
    PriorityTypeId = 2,
    LocationId    = 5
});

// Create ticket (display names — requires InitializeMetadataCacheAsync first)
var created = await client.CreateTicketAsync(new NamedCreateTicketRequest
{
    Subject      = "Printer is broken",
    ClientId     = 101,
    PriorityType = "High",
    RequestType  = "IT Support"
});

// Update ticket (only provided fields are sent)
var updated = await client.Tickets.UpdateAsync(1234, new UpdateTicketRequest
{
    StatusTypeId = 3,   // e.g. "Resolved"
    TechId       = 7
});

// Update ticket (by name)
await client.UpdateTicketAsync(1234, new NamedUpdateTicketRequest
{
    StatusType = "Resolved",
    TechId     = 7
});

// Delete ticket
await client.Tickets.DeleteAsync(1234);
```

### Clients (End Users)

```csharp
// List clients
var clients = await client.Clients.ListAsync(new PaginationOptions { Limit = 25 });

// Get a specific client
var helpDeskClient = await client.Clients.GetAsync(101);

// Search clients by name / username / email
var results = await client.Clients.SearchAsync("john.doe");
```

### Assets

```csharp
// List assets
var assets = await client.Assets.ListAsync();

// Get a specific asset
var asset = await client.Assets.GetAsync(200);

// Search assets
var results = await client.Assets.SearchAsync("HP LaserJet");
```

### Locations

```csharp
// List all locations
var locations = await client.Locations.ListAsync();

// Get a specific location
var loc = await client.Locations.GetAsync(5);
```

### Request Types (Categories)

```csharp
// List all request types
var types = await client.RequestTypes.ListAsync();

// Get a specific request type
var rt = await client.RequestTypes.GetAsync(10);
```

### Ticket Notes

```csharp
// List all notes for a ticket
var notes = await client.Notes.ListByTicketAsync(ticketId: 1234);

// Get a specific note
var note = await client.Notes.GetAsync(noteId: 55);

// Add a note to a ticket
var newNote = await client.Notes.CreateAsync(new CreateNoteRequest
{
    TicketId      = 1234,
    NoteText      = "Replaced the toner cartridge. Printer is operational.",
    IsClientNote  = true,    // visible to the client
    IsSolutionNote = true
});
```

### Lookup Data

```csharp
// Get all status types (e.g. Open, Closed, Resolved, …)
var statuses = await client.Lookups.GetStatusTypesAsync();

// Get all priority types (e.g. Critical, High, Medium, Low)
var priorities = await client.Lookups.GetPriorityTypesAsync();

// Get all technicians
var techs = await client.Lookups.GetTechsAsync();
```

---

## Pagination

Most list endpoints accept a `PaginationOptions` parameter:

```csharp
// Fetch page 3 of assets (0-based offset, 50 per page)
var page3 = await client.Assets.ListAsync(new PaginationOptions
{
    Limit = 50,
    Start = 100   // 50 * (pageNumber - 1)
});
```

The WHD API uses `limit` and `start` query parameters (not `page`/`perPage`).
Keep requesting pages until the returned list is empty or shorter than `Limit`.

---

## Error Handling

A failed HTTP response (any non-2xx status code) throws a `WhdApiException`:

```csharp
using LS.WHD.Client.Exceptions;

try
{
    var ticket = await client.Tickets.GetAsync(999999);
}
catch (WhdApiException ex) when (ex.StatusCode == 404)
{
    Console.WriteLine("Ticket not found.");
}
catch (WhdApiException ex)
{
    Console.WriteLine($"API error {ex.StatusCode}: {ex.Message}");
}
```

---

## API Base Path

The client communicates with the classic WHD REST endpoint:

```
{BaseUrl}/helpdesk/WebObjects/HELPDESK.woa/ra/{Resource}
```

This is the primary REST API path documented in the SolarWinds Web Help Desk REST API guide.

---

## Self-Signed / Internal Certificates

In development environments with self-signed certificates, set `IgnoreSslErrors = true`:

```csharp
var options = new WhdClientOptions
{
    BaseUrl = "https://internal-whd-server:8443",
    ApiKey  = "key",
    IgnoreSslErrors = true   // ⚠ do not use in production
};
```

---

## API Limitations

The SolarWinds Web Help Desk REST API has some documented limitations that are reflected in this client's design:

- **Attachments**: The `TicketNotes` resource supports note creation; binary attachment upload is not officially documented in the REST API and is therefore **not implemented**.
- **Filtering**: The API supports `limit`, `start`, and `search` parameters. Complex server-side filtering (e.g. by date range, by status) is not uniformly documented and therefore **not included**; filter results client-side where needed.
- **Create/Update Clients and Assets**: The WHD REST API exposes these objects as read-only in its public documentation; write operations for clients and assets are **not implemented**.

---

## Project Structure

```
src/
  LS.WHD.Client/
    Authentication/   – AuthMode enum
    Exceptions/       – WhdApiException, WhdMetadataException
    Http/             – WhdHttpClient, WhdHttpClientFactory, QueryParameters
    Metadata/         – IWhdMetadataCache, WhdMetadataCache (name→ID resolution)
    Models/           – Request/response DTOs (incl. NamedCreateTicketRequest, NamedUpdateTicketRequest)
    Services/         – ITicketService, IClientService, IAssetService, …
    WhdClient.cs      – Main entry point
    WhdClientOptions.cs
  LS.WHD.Client.Mcp/
    Tools/            – MCP tool classes (TicketTools, ClientTools, AssetTools, …)
    Program.cs        – Host builder + STDIO transport
    WhdConfiguration.cs – Reads env vars and builds WhdClientOptions
    WhdClientService.cs – Singleton wrapper with lazy metadata initialization

tests/
  LS.WHD.Client.Tests/
    MetadataCacheTests.cs
    SerializationTests.cs
    WhdHttpClientTests.cs
    QueryParametersTests.cs
    PaginationOptionsTests.cs
    WhdApiExceptionTests.cs
```

---

## MCP Server (`LS.WHD.Client.Mcp`)

`LS.WHD.Client.Mcp` is a **Model Context Protocol (MCP) server** that wraps the
`LS.WHD.Client` library and exposes WHD operations as MCP tools. LLM clients (such as
Claude Desktop, VS Code Copilot Chat, and any MCP-compatible host) can connect to it and
call WHD operations directly from chat or agent workflows.

### What is MCP?

The [Model Context Protocol](https://modelcontextprotocol.io/) is an open standard that
lets AI assistants talk to external tools and data sources in a structured, type-safe way.

### Configuration

All settings are read from environment variables at startup.

| Variable | Required | Description |
|---|---|---|
| `WHD_BASE_URL` | ✅ | Base URL of the WHD server, e.g. `https://helpdesk.example.com:8081` |
| `WHD_AUTH_MODE` | | `ApiKey` (default) or `BasicAuth` |
| `WHD_API_KEY` | | API key when `WHD_AUTH_MODE=ApiKey` |
| `WHD_USERNAME` | | Username when `WHD_AUTH_MODE=BasicAuth` |
| `WHD_PASSWORD` | | Password when `WHD_AUTH_MODE=BasicAuth` |
| `WHD_IGNORE_SSL_ERRORS` | | `true` to skip TLS validation (development only) |

### How to run

**From source:**

```shell
cd src/LS.WHD.Client.Mcp

# ApiKey auth
WHD_BASE_URL=https://helpdesk.example.com:8081 \
WHD_API_KEY=your-api-key \
dotnet run
```

**Published executable:**

```shell
dotnet publish src/LS.WHD.Client.Mcp -c Release -o ./publish/mcp

WHD_BASE_URL=https://helpdesk.example.com:8081 \
WHD_API_KEY=your-api-key \
./publish/mcp/LS.WHD.Client.Mcp
```

The server communicates over **STDIO** (standard input / output), which is the conventional
transport for locally-launched MCP servers.

### Wiring into Claude Desktop

Add an entry to your `claude_desktop_config.json`:

```json
{
  "mcpServers": {
    "whd": {
      "command": "dotnet",
      "args": ["run", "--project", "/path/to/src/LS.WHD.Client.Mcp"],
      "env": {
        "WHD_BASE_URL": "https://helpdesk.example.com:8081",
        "WHD_AUTH_MODE": "ApiKey",
        "WHD_API_KEY": "your-api-key"
      }
    }
  }
}
```

Or, using a published binary instead of `dotnet run`:

```json
{
  "mcpServers": {
    "whd": {
      "command": "/path/to/publish/mcp/LS.WHD.Client.Mcp",
      "env": {
        "WHD_BASE_URL": "https://helpdesk.example.com:8081",
        "WHD_AUTH_MODE": "ApiKey",
        "WHD_API_KEY": "your-api-key"
      }
    }
  }
}
```

### Available tools

| Tool | Description |
|---|---|
| `list_tickets` | Returns a paged list of tickets |
| `get_ticket` | Returns a single ticket by ID |
| `create_ticket` | Creates a new ticket (priority / request type accept display names) |
| `update_ticket` | Updates an existing ticket (status / priority / request type accept display names) |
| `list_clients` | Returns a paged list of client (end-user) accounts |
| `search_clients` | Searches clients by username or email |
| `list_assets` | Returns a paged list of assets |
| `search_assets` | Searches assets by name or tag |
| `list_locations` | Returns all configured locations |
| `list_request_types` | Returns all configured request type categories |
| `add_ticket_note` | Adds a note / comment to a ticket |
| `list_ticket_notes` | Returns all notes for a ticket |
| `list_priorities` | Returns all priority types |
| `list_status_types` | Returns all status types |
| `list_technicians` | Returns a paged list of technician accounts |

**LLM-friendly design:** `create_ticket` and `update_ticket` accept priority, request type,
and status values as **display names** (e.g. `"High"`, `"IT Support"`, `"Resolved"`) in
addition to numeric IDs. The server resolves names to IDs automatically using the metadata
cache, which is populated lazily on first use.

---

## Building

```shell
dotnet build LS.WHD.Client.slnx
dotnet test  LS.WHD.Client.slnx
```

The solution contains both the client library (`LS.WHD.Client`) and the MCP server
(`LS.WHD.Client.Mcp`). Running the commands above builds and tests both.

---

## License

MIT
