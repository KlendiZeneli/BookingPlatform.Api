using System;
using System.Collections.Generic;
using System.Text;

namespace BookingPlatform.Domain.Entities;

public class User : BaseEntity
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public string? ProfileImageUrl { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public OwnerProfile? OwnerProfile { get; set; }

    public ICollection<Property> OwnedProperties { get; set; } = new List<Property>();
    public ICollection<Booking> GuestBookings { get; set; } = new List<Booking>();
    
    // password reset fields
    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetExpires { get; set; }
}

