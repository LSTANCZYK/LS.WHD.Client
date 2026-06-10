namespace LS.WHD.Client.Models;

/// <summary>
/// Represents a technician account in Web Help Desk.
/// </summary>
public sealed class Tech
{
    /// <summary>Internal unique identifier.</summary>
    public long Id { get; set; }

    /// <summary>Login username of the technician.</summary>
    public string? UserName { get; set; }

    /// <summary>First name.</summary>
    public string? FirstName { get; set; }

    /// <summary>Last name.</summary>
    public string? LastName { get; set; }

    /// <summary>Email address.</summary>
    public string? Email { get; set; }

    /// <summary>Whether the technician account is active.</summary>
    public bool? IsActive { get; set; }
}
