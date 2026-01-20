namespace UserService.Models;

public class UserProfile
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? ProfileImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<Address> Addresses { get; set; } = new List<Address>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}

public class Address
{
    public Guid Id { get; set; }
    public Guid UserProfileId { get; set; }
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }

    public UserProfile UserProfile { get; set; } = null!;
}

public class Notification
{
    public Guid Id { get; set; }
    public Guid UserProfileId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }

    public UserProfile UserProfile { get; set; } = null!;
}
