namespace LS.WHD.Client.Models;

/// <summary>
/// Represents an asset (hardware or software item) tracked in Web Help Desk.
/// </summary>
public sealed class Asset
{
    /// <summary>Internal unique identifier.</summary>
    public long Id { get; set; }

    /// <summary>Asset name or label.</summary>
    public string? Name { get; set; }

    /// <summary>Asset tag / inventory number.</summary>
    public string? AssetTag { get; set; }

    /// <summary>Serial number of the asset.</summary>
    public string? SerialNumber { get; set; }

    /// <summary>Manufacturer of the asset.</summary>
    public string? Manufacturer { get; set; }

    /// <summary>Model name or number.</summary>
    public string? Model { get; set; }

    /// <summary>Asset type (e.g. "Laptop", "Desktop").</summary>
    public WhdRef? AssetType { get; set; }

    /// <summary>Current status (e.g. "Active", "Disposed").</summary>
    public WhdRef? StatusType { get; set; }

    /// <summary>Location where the asset is situated.</summary>
    public WhdRef? Location { get; set; }

    /// <summary>Client currently assigned to / owning this asset.</summary>
    public WhdRef? Client { get; set; }

    /// <summary>Date the asset was purchased.</summary>
    public DateTimeOffset? PurchaseDate { get; set; }

    /// <summary>Warranty expiration date.</summary>
    public DateTimeOffset? WarrantyExpirationDate { get; set; }

    /// <summary>Date the asset was last updated in WHD.</summary>
    public DateTimeOffset? LastUpdated { get; set; }
}
