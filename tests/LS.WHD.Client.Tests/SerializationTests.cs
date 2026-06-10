using System.Text.Json;
using LS.WHD.Client.Models;

namespace LS.WHD.Client.Tests;

/// <summary>
/// Tests that model DTOs round-trip through JSON serialization correctly.
/// </summary>
public class SerializationTests
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    [Fact]
    public void Ticket_RoundTrip_PreservesAllFields()
    {
        var ticket = new Ticket
        {
            Id = 42,
            Subject = "Printer broken",
            Detail = "Toner low",
            StatusType = new WhdRef { Id = 1, Name = "Open" },
            PriorityType = new WhdRef { Id = 2, Name = "High" },
            RequestType = new WhdRef { Id = 10, Name = "IT Support" },
            Client = new WhdRef { Id = 101, Name = "Jane Smith" },
            Location = new WhdRef { Id = 5, Name = "Main Office" },
            CreatedDate = new DateTimeOffset(2024, 1, 15, 8, 0, 0, TimeSpan.Zero),
            DueDate = new DateTimeOffset(2024, 1, 20, 17, 0, 0, TimeSpan.Zero)
        };

        var json = JsonSerializer.Serialize(ticket, Options);
        var deserialized = JsonSerializer.Deserialize<Ticket>(json, Options);

        Assert.NotNull(deserialized);
        Assert.Equal(42, deserialized.Id);
        Assert.Equal("Printer broken", deserialized.Subject);
        Assert.Equal("Toner low", deserialized.Detail);
        Assert.Equal(1, deserialized.StatusType?.Id);
        Assert.Equal("Open", deserialized.StatusType?.Name);
        Assert.Equal(2, deserialized.PriorityType?.Id);
        Assert.Equal(10, deserialized.RequestType?.Id);
        Assert.Equal(101, deserialized.Client?.Id);
        Assert.Equal(5, deserialized.Location?.Id);
        Assert.Equal(new DateTimeOffset(2024, 1, 15, 8, 0, 0, TimeSpan.Zero), deserialized.CreatedDate);
    }

    [Fact]
    public void Ticket_NullOptionalFields_NotSerializedInJson()
    {
        var ticket = new Ticket { Id = 1, Subject = "Test" };

        var json = JsonSerializer.Serialize(ticket, Options);

        Assert.DoesNotContain("\"detail\"", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("\"dueDate\"", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("\"asset\"", json, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void WhdClient_RoundTrip_PreservesAllFields()
    {
        var client = new HelpDeskClient
        {
            Id = 99,
            UserName = "jdoe",
            FirstName = "John",
            LastName = "Doe",
            Email = "jdoe@example.com",
            Phone = "555-1234",
            Location = new WhdRef { Id = 3, Name = "Branch A" },
            IsActive = true
        };

        var json = JsonSerializer.Serialize(client, Options);
        var deserialized = JsonSerializer.Deserialize<HelpDeskClient>(json, Options);

        Assert.NotNull(deserialized);
        Assert.Equal(99, deserialized.Id);
        Assert.Equal("jdoe", deserialized.UserName);
        Assert.Equal("John", deserialized.FirstName);
        Assert.Equal("Doe", deserialized.LastName);
        Assert.Equal("jdoe@example.com", deserialized.Email);
        Assert.True(deserialized.IsActive);
        Assert.Equal(3, deserialized.Location?.Id);
    }

    [Fact]
    public void Asset_RoundTrip_PreservesAllFields()
    {
        var asset = new Asset
        {
            Id = 200,
            Name = "Dell Laptop",
            AssetTag = "ASSET-001",
            SerialNumber = "SN123456",
            Manufacturer = "Dell",
            Model = "Latitude 5520",
            AssetType = new WhdRef { Id = 1, Name = "Laptop" },
            StatusType = new WhdRef { Id = 1, Name = "Active" },
            Location = new WhdRef { Id = 5, Name = "Main Office" }
        };

        var json = JsonSerializer.Serialize(asset, Options);
        var deserialized = JsonSerializer.Deserialize<Asset>(json, Options);

        Assert.NotNull(deserialized);
        Assert.Equal(200, deserialized.Id);
        Assert.Equal("Dell Laptop", deserialized.Name);
        Assert.Equal("ASSET-001", deserialized.AssetTag);
        Assert.Equal("SN123456", deserialized.SerialNumber);
        Assert.Equal("Dell", deserialized.Manufacturer);
        Assert.Equal("Latitude 5520", deserialized.Model);
    }

    [Fact]
    public void Location_RoundTrip_PreservesAllFields()
    {
        var location = new Location
        {
            Id = 5,
            Name = "Main Office",
            Address1 = "123 Main St",
            City = "Springfield",
            State = "IL",
            Zip = "62701",
            Country = "USA"
        };

        var json = JsonSerializer.Serialize(location, Options);
        var deserialized = JsonSerializer.Deserialize<Location>(json, Options);

        Assert.NotNull(deserialized);
        Assert.Equal(5, deserialized.Id);
        Assert.Equal("Main Office", deserialized.Name);
        Assert.Equal("123 Main St", deserialized.Address1);
        Assert.Equal("Springfield", deserialized.City);
    }

    [Fact]
    public void RequestType_WithParent_RoundTrip()
    {
        var requestType = new RequestType
        {
            Id = 12,
            Name = "Printer Issues",
            Parent = new WhdRef { Id = 10, Name = "IT Support" }
        };

        var json = JsonSerializer.Serialize(requestType, Options);
        var deserialized = JsonSerializer.Deserialize<RequestType>(json, Options);

        Assert.NotNull(deserialized);
        Assert.Equal(12, deserialized.Id);
        Assert.Equal("Printer Issues", deserialized.Name);
        Assert.NotNull(deserialized.Parent);
        Assert.Equal(10, deserialized.Parent.Id);
        Assert.Equal("IT Support", deserialized.Parent.Name);
    }

    [Fact]
    public void TicketNote_RoundTrip_PreservesAllFields()
    {
        var note = new TicketNote
        {
            Id = 55,
            NoteText = "Checked the printer; toner cartridge replaced.",
            IsClientNote = true,
            IsSolutionNote = false,
            Author = new WhdRef { Id = 7, Name = "Tech John" },
            CreatedDate = new DateTimeOffset(2024, 2, 1, 10, 30, 0, TimeSpan.Zero)
        };

        var json = JsonSerializer.Serialize(note, Options);
        var deserialized = JsonSerializer.Deserialize<TicketNote>(json, Options);

        Assert.NotNull(deserialized);
        Assert.Equal(55, deserialized.Id);
        Assert.Equal("Checked the printer; toner cartridge replaced.", deserialized.NoteText);
        Assert.True(deserialized.IsClientNote);
        Assert.False(deserialized.IsSolutionNote);
        Assert.Equal(7, deserialized.Author?.Id);
    }

    [Fact]
    public void CreateTicketRequest_Serializes_RequiredFields()
    {
        var request = new CreateTicketRequest
        {
            Subject = "Monitor not working",
            Detail = "Screen goes black after boot",
            ClientId = 101,
            RequestTypeId = 10,
            PriorityTypeId = 2
        };

        var json = JsonSerializer.Serialize(request, Options);

        Assert.Contains("Monitor not working", json);
        Assert.Contains("Screen goes black after boot", json);
        Assert.DoesNotContain("\"assetId\"", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("\"locationId\"", json, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void WhdRef_RoundTrip()
    {
        var @ref = new WhdRef { Id = 7, Name = "Some Entity" };

        var json = JsonSerializer.Serialize(@ref, Options);
        var deserialized = JsonSerializer.Deserialize<WhdRef>(json, Options);

        Assert.NotNull(deserialized);
        Assert.Equal(7, deserialized.Id);
        Assert.Equal("Some Entity", deserialized.Name);
    }

    [Fact]
    public void DeserializeFromApi_CaseInsensitive()
    {
        // Simulate WHD API returning mixed-case JSON keys
        const string json = """
            {
                "id": 99,
                "userName": "jdoe",
                "firstName": "John",
                "lastName": "Doe",
                "email": "jdoe@example.com",
                "location": { "id": 5, "name": "Main Office" }
            }
            """;

        var deserialized = JsonSerializer.Deserialize<HelpDeskClient>(json, Options);

        Assert.NotNull(deserialized);
        Assert.Equal(99, deserialized.Id);
        Assert.Equal("jdoe", deserialized.UserName);
        Assert.Equal(5, deserialized.Location?.Id);
    }
}
