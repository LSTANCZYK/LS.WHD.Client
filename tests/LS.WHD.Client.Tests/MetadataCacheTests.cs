using LS.WHD.Client.Exceptions;
using LS.WHD.Client.Metadata;
using LS.WHD.Client.Models;
using LS.WHD.Client.Services;
using Moq;

namespace LS.WHD.Client.Tests;

/// <summary>
/// Tests for <see cref="WhdMetadataCache"/> — the in-memory name→ID resolver.
/// </summary>
public class MetadataCacheTests
{
    // ──────────────── helpers ────────────────

    /// <summary>
    /// Creates a <see cref="WhdMetadataCache"/> whose backing services return the supplied data.
    /// </summary>
    private static WhdMetadataCache BuildCache(
        IReadOnlyList<PriorityType>?          priorities   = null,
        IReadOnlyList<StatusType>?            statuses     = null,
        IReadOnlyList<RequestType>?           requestTypes = null,
        IReadOnlyList<CustomFieldDefinition>? customFields = null)
    {
        var lookups        = new Mock<ILookupService>();
        var requestTypeSvc = new Mock<IRequestTypeService>();
        var customFieldSvc = new Mock<ICustomFieldService>();

        lookups.Setup(x => x.GetPriorityTypesAsync(It.IsAny<CancellationToken>()))
               .ReturnsAsync(priorities ?? []);

        lookups.Setup(x => x.GetStatusTypesAsync(It.IsAny<CancellationToken>()))
               .ReturnsAsync(statuses ?? []);

        requestTypeSvc
            .Setup(x => x.ListAsync(It.IsAny<PaginationOptions?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(requestTypes ?? []);

        customFieldSvc.Setup(x => x.ListAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(customFields ?? []);

        return new WhdMetadataCache(lookups.Object, requestTypeSvc.Object, customFieldSvc.Object);
    }

    // ──────────────── IsInitialized ────────────────

    [Fact]
    public void IsInitialized_BeforeInit_ReturnsFalse()
    {
        var cache = BuildCache();
        Assert.False(cache.IsInitialized);
    }

    [Fact]
    public async Task IsInitialized_AfterInit_ReturnsTrue()
    {
        var cache = BuildCache();
        await cache.InitializeAsync();
        Assert.True(cache.IsInitialized);
    }

    // ──────────────── Not-initialized guard ────────────────

    [Fact]
    public void ResolvePriority_WhenNotInitialized_Throws()
    {
        var cache = BuildCache();
        var ex = Assert.Throws<InvalidOperationException>(
            () => cache.ResolvePriorityId("High"));
        Assert.Contains("InitializeMetadataCacheAsync", ex.Message);
    }

    [Fact]
    public void ResolveRequestType_WhenNotInitialized_Throws()
    {
        var cache = BuildCache();
        Assert.Throws<InvalidOperationException>(() => cache.ResolveRequestTypeId("IT Support"));
    }

    [Fact]
    public void ResolveCustomField_WhenNotInitialized_Throws()
    {
        var cache = BuildCache();
        Assert.Throws<InvalidOperationException>(() => cache.ResolveCustomFieldId("Department"));
    }

    [Fact]
    public void ResolveStatus_WhenNotInitialized_Throws()
    {
        var cache = BuildCache();
        Assert.Throws<InvalidOperationException>(() => cache.ResolveStatusTypeId("Open"));
    }

    // ──────────────── Numeric ID pass-through (no cache needed) ────────────────

    [Fact]
    public void ResolvePriority_NumericString_ReturnsIdWithoutInit()
    {
        var cache = BuildCache();
        Assert.Equal(42L, cache.ResolvePriorityId("42"));
    }

    [Fact]
    public void ResolveRequestType_NumericString_ReturnsIdWithoutInit()
    {
        var cache = BuildCache();
        Assert.Equal(10L, cache.ResolveRequestTypeId("10"));
    }

    [Fact]
    public void ResolveCustomField_NumericString_ReturnsIdWithoutInit()
    {
        var cache = BuildCache();
        Assert.Equal(3L, cache.ResolveCustomFieldId("3"));
    }

    [Fact]
    public void ResolveStatus_NumericString_ReturnsIdWithoutInit()
    {
        var cache = BuildCache();
        Assert.Equal(5L, cache.ResolveStatusTypeId("5"));
    }

    // ──────────────── Resolve by name (after init) ────────────────

    [Fact]
    public async Task ResolvePriority_ByName_ReturnsCorrectId()
    {
        var cache = BuildCache(priorities:
        [
            new PriorityType { Id = 1, Name = "Critical" },
            new PriorityType { Id = 2, Name = "High" },
            new PriorityType { Id = 3, Name = "Medium" }
        ]);

        await cache.InitializeAsync();

        Assert.Equal(1L, cache.ResolvePriorityId("Critical"));
        Assert.Equal(2L, cache.ResolvePriorityId("High"));
        Assert.Equal(3L, cache.ResolvePriorityId("Medium"));
    }

    [Fact]
    public async Task ResolveRequestType_ByName_ReturnsCorrectId()
    {
        var cache = BuildCache(requestTypes:
        [
            new RequestType { Id = 10, Name = "IT Support" },
            new RequestType { Id = 20, Name = "Hardware" }
        ]);

        await cache.InitializeAsync();

        Assert.Equal(10L, cache.ResolveRequestTypeId("IT Support"));
        Assert.Equal(20L, cache.ResolveRequestTypeId("Hardware"));
    }

    [Fact]
    public async Task ResolveCustomField_ByLabel_ReturnsCorrectId()
    {
        var cache = BuildCache(customFields:
        [
            new CustomFieldDefinition { Id = 3, FieldLabel = "Department" },
            new CustomFieldDefinition { Id = 7, FieldLabel = "Cost Center" }
        ]);

        await cache.InitializeAsync();

        Assert.Equal(3L, cache.ResolveCustomFieldId("Department"));
        Assert.Equal(7L, cache.ResolveCustomFieldId("Cost Center"));
    }

    [Fact]
    public async Task ResolveStatus_ByName_ReturnsCorrectId()
    {
        var cache = BuildCache(statuses:
        [
            new StatusType { Id = 1, Name = "Open" },
            new StatusType { Id = 2, Name = "Resolved" }
        ]);

        await cache.InitializeAsync();

        Assert.Equal(1L, cache.ResolveStatusTypeId("Open"));
        Assert.Equal(2L, cache.ResolveStatusTypeId("Resolved"));
    }

    // ──────────────── Name resolution is case-insensitive ────────────────

    [Fact]
    public async Task ResolvePriority_NameCaseInsensitive()
    {
        var cache = BuildCache(priorities:
        [
            new PriorityType { Id = 2, Name = "High" }
        ]);

        await cache.InitializeAsync();

        Assert.Equal(2L, cache.ResolvePriorityId("HIGH"));
        Assert.Equal(2L, cache.ResolvePriorityId("high"));
        Assert.Equal(2L, cache.ResolvePriorityId("High"));
    }

    [Fact]
    public async Task ResolveCustomField_LabelCaseInsensitive()
    {
        var cache = BuildCache(customFields:
        [
            new CustomFieldDefinition { Id = 3, FieldLabel = "Department" }
        ]);

        await cache.InitializeAsync();

        Assert.Equal(3L, cache.ResolveCustomFieldId("department"));
        Assert.Equal(3L, cache.ResolveCustomFieldId("DEPARTMENT"));
    }

    // ──────────────── Numeric string also works after init ────────────────

    [Fact]
    public async Task ResolvePriority_NumericStringAfterInit_ReturnsNumericValue()
    {
        var cache = BuildCache(priorities:
        [
            new PriorityType { Id = 2, Name = "High" }
        ]);

        await cache.InitializeAsync();

        // "2" should be treated as the numeric ID directly, not searched by name
        Assert.Equal(2L, cache.ResolvePriorityId("2"));
    }

    // ──────────────── Not-found errors ────────────────

    [Fact]
    public async Task ResolvePriority_UnknownName_ThrowsWhdMetadataException()
    {
        var cache = BuildCache(priorities:
        [
            new PriorityType { Id = 2, Name = "High" }
        ]);

        await cache.InitializeAsync();

        var ex = Assert.Throws<WhdMetadataException>(
            () => cache.ResolvePriorityId("Urgent"));

        Assert.Equal("Priority", ex.MetadataKind);
        Assert.Equal("Urgent", ex.LookupValue);
        Assert.Contains("Urgent", ex.Message);
    }

    [Fact]
    public async Task ResolveRequestType_UnknownName_ThrowsWhdMetadataException()
    {
        var cache = BuildCache(requestTypes:
        [
            new RequestType { Id = 10, Name = "IT Support" }
        ]);

        await cache.InitializeAsync();

        var ex = Assert.Throws<WhdMetadataException>(
            () => cache.ResolveRequestTypeId("Facilities"));

        Assert.Equal("RequestType", ex.MetadataKind);
        Assert.Equal("Facilities", ex.LookupValue);
    }

    [Fact]
    public async Task ResolveCustomField_UnknownLabel_ThrowsWhdMetadataException()
    {
        var cache = BuildCache(customFields:
        [
            new CustomFieldDefinition { Id = 3, FieldLabel = "Department" }
        ]);

        await cache.InitializeAsync();

        Assert.Throws<WhdMetadataException>(
            () => cache.ResolveCustomFieldId("Project Code"));
    }

    // ──────────────── Ambiguous name errors ────────────────

    [Fact]
    public async Task ResolvePriority_DuplicateName_ThrowsWhdMetadataException()
    {
        var cache = BuildCache(priorities:
        [
            new PriorityType { Id = 2,  Name = "High" },
            new PriorityType { Id = 99, Name = "High" }   // duplicate
        ]);

        await cache.InitializeAsync();

        var ex = Assert.Throws<WhdMetadataException>(
            () => cache.ResolvePriorityId("High"));

        Assert.Equal("Priority", ex.MetadataKind);
        Assert.Contains("ambiguous", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("numeric ID", ex.Message);
    }

    [Fact]
    public async Task ResolveRequestType_DuplicateName_ThrowsWhdMetadataException()
    {
        var cache = BuildCache(requestTypes:
        [
            new RequestType { Id = 10, Name = "Support" },
            new RequestType { Id = 11, Name = "Support" }
        ]);

        await cache.InitializeAsync();

        var ex = Assert.Throws<WhdMetadataException>(
            () => cache.ResolveRequestTypeId("Support"));

        Assert.Equal("RequestType", ex.MetadataKind);
        Assert.Contains("ambiguous", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ResolveCustomField_DuplicateLabel_ThrowsWhdMetadataException()
    {
        var cache = BuildCache(customFields:
        [
            new CustomFieldDefinition { Id = 3, FieldLabel = "Notes" },
            new CustomFieldDefinition { Id = 9, FieldLabel = "Notes" }
        ]);

        await cache.InitializeAsync();

        Assert.Throws<WhdMetadataException>(
            () => cache.ResolveCustomFieldId("Notes"));
    }

    // ──────────────── Ambiguous by name but reachable by numeric ID ────────────────

    [Fact]
    public async Task ResolvePriority_DuplicateName_NumericIdStillWorks()
    {
        var cache = BuildCache(priorities:
        [
            new PriorityType { Id = 2,  Name = "High" },
            new PriorityType { Id = 99, Name = "High" }
        ]);

        await cache.InitializeAsync();

        // Ambiguous by name, but numeric strings are always accepted
        Assert.Equal(2L,  cache.ResolvePriorityId("2"));
        Assert.Equal(99L, cache.ResolvePriorityId("99"));
    }

    // ──────────────── Null/empty names are skipped ────────────────

    [Fact]
    public async Task ResolvePriority_NullNameEntries_AreIgnored()
    {
        var cache = BuildCache(priorities:
        [
            new PriorityType { Id = 1, Name = null },
            new PriorityType { Id = 2, Name = "High" }
        ]);

        await cache.InitializeAsync();

        Assert.Equal(2L, cache.ResolvePriorityId("High"));
    }

    // ──────────────── Refresh re-loads the cache ────────────────

    [Fact]
    public async Task RefreshAsync_ReplacesCache_WithNewData()
    {
        var lookups        = new Mock<ILookupService>();
        var requestTypeSvc = new Mock<IRequestTypeService>();
        var customFieldSvc = new Mock<ICustomFieldService>();

        // First load: only "Low"
        lookups.Setup(x => x.GetPriorityTypesAsync(It.IsAny<CancellationToken>()))
               .ReturnsAsync([new PriorityType { Id = 1, Name = "Low" }]);
        lookups.Setup(x => x.GetStatusTypesAsync(It.IsAny<CancellationToken>()))
               .ReturnsAsync([]);
        requestTypeSvc
            .Setup(x => x.ListAsync(It.IsAny<PaginationOptions?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        customFieldSvc.Setup(x => x.ListAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);

        var cache = new WhdMetadataCache(lookups.Object, requestTypeSvc.Object, customFieldSvc.Object);
        await cache.InitializeAsync();

        Assert.Equal(1L, cache.ResolvePriorityId("Low"));

        // Simulate WHD adding "High" priority
        lookups.Setup(x => x.GetPriorityTypesAsync(It.IsAny<CancellationToken>()))
               .ReturnsAsync(
               [
                   new PriorityType { Id = 1, Name = "Low"  },
                   new PriorityType { Id = 2, Name = "High" }
               ]);

        await cache.RefreshAsync();

        Assert.Equal(2L, cache.ResolvePriorityId("High"));
        Assert.Equal(1L, cache.ResolvePriorityId("Low"));   // old entry still present
    }

    [Fact]
    public async Task InitializeAsync_CalledTwice_RefreshesCache()
    {
        var lookups        = new Mock<ILookupService>();
        var requestTypeSvc = new Mock<IRequestTypeService>();
        var customFieldSvc = new Mock<ICustomFieldService>();

        var callCount = 0;
        lookups.Setup(x => x.GetPriorityTypesAsync(It.IsAny<CancellationToken>()))
               .ReturnsAsync(() =>
               {
                   callCount++;
                   return callCount == 1
                       ? (IReadOnlyList<PriorityType>) []
                       : [new PriorityType { Id = 5, Name = "Critical" }];
               });
        lookups.Setup(x => x.GetStatusTypesAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);
        requestTypeSvc
            .Setup(x => x.ListAsync(It.IsAny<PaginationOptions?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        customFieldSvc.Setup(x => x.ListAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);

        var cache = new WhdMetadataCache(lookups.Object, requestTypeSvc.Object, customFieldSvc.Object);

        await cache.InitializeAsync();   // first: empty
        await cache.InitializeAsync();   // second: picks up "Critical"

        Assert.Equal(5L, cache.ResolvePriorityId("Critical"));
    }
}

