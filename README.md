# LS.WHD.Client

A modern, idiomatic C# client library for the **SolarWinds Web Help Desk (WHD) REST API**.

- Targets **.NET 8**
- Uses `HttpClient` and `async`/`await` throughout
- Supports **API Key** and **Basic Auth** authentication
- Covers tickets, clients, assets, locations, request types, notes, and lookup data
- Handles pagination and error responses

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

## Supported Operations

### Tickets

```csharp
// List tickets (paged)
var tickets = await client.Tickets.ListAsync(
    new PaginationOptions { Limit = 50, Start = 0 });

// Get single ticket
var ticket = await client.Tickets.GetAsync(1234);

// Create ticket
var created = await client.Tickets.CreateAsync(new CreateTicketRequest
{
    Subject       = "Printer is broken",
    Detail        = "Office printer on floor 2 prints blank pages.",
    ClientId      = 101,
    RequestTypeId = 10,
    PriorityTypeId = 2,
    LocationId    = 5
});

// Update ticket (only provided fields are sent)
var updated = await client.Tickets.UpdateAsync(1234, new UpdateTicketRequest
{
    StatusTypeId = 3,   // e.g. "Resolved"
    TechId       = 7
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
    Exceptions/       – WhdApiException
    Http/             – WhdHttpClient, WhdHttpClientFactory, QueryParameters
    Models/           – Request/response DTOs
    Services/         – ITicketService, IClientService, IAssetService, …
    WhdClient.cs      – Main entry point
    WhdClientOptions.cs

tests/
  LS.WHD.Client.Tests/
    SerializationTests.cs
    WhdHttpClientTests.cs
    QueryParametersTests.cs
    PaginationOptionsTests.cs
    WhdApiExceptionTests.cs
```

---

## Building

```shell
dotnet build LS.WHD.Client.sln
dotnet test  LS.WHD.Client.sln
```

---

## License

MIT
