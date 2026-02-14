using System;
using System.Collections.Generic;
using System.Text;

namespace BookingPlatform.Domain.Entities;

public class Property : BaseEntity
{
    public Guid OwnerId { get; set; }
    public User Owner { get; set; } = default!;

    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string PropertyType { get; set; } = default!;

    public Guid AddressId { get; set; }
    public Address Address { get; set; } = default!;

    public int MaxGuests { get; set; }
    public TimeOnly CheckInTime { get; set; }
    public TimeOnly CheckOutTime { get; set; }

    public bool IsActive { get; set; }
    public bool IsApproved { get; set; }

    public DateTime? LastBookedOnUtc { get; set; }

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
