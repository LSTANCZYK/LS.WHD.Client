namespace LS.WHD.Client.Models;

/// <summary>
/// Represents a client (end user / requester) in Web Help Desk.
/// </summary>
public sealed class HelpDeskClient
{
    /// <summary>Internal unique identifier.</summary>
    public long Id { get; set; }

    /// <summary>Login username.</summary>
    public string? UserName { get; set; }

    /// <summary>First name.</summary>
    public string? FirstName { get; set; }

    /// <summary>Last name.</summary>
    public string? LastName { get; set; }

    /// <summary>Primary email address.</summary>
    public string? Email { get; set; }

    /// <summary>Phone number.</summary>
    public string? Phone { get; set; }

    /// <summary>Office / desk phone number.</summary>
    public string? WorkPhone { get; set; }

    /// <summary>Mobile phone number.</summary>
    public string? MobilePhone { get; set; }

    /// <summary>Location the client belongs to.</summary>
    public WhdRef? Location { get; set; }

    /// <summary>Whether the client account is active.</summary>
    public bool? IsActive { get; set; }

    /// <summary>Department the client belongs to.</summary>
    public string? Department { get; set; }
}
