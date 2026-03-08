using System;
using System.Collections.Generic;
using System.Text;
using BookingPlatform.Domain.Enums;

namespace BookingPlatform.Domain.Entities;

public class Property : BaseEntity
{
    public Guid OwnerProfileId { get; set; }
    public OwnerProfile OwnerProfile { get; set; } = default!;

    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public PropertyTypeEnum PropertyType { get; set; } = default!;
    public Guid AddressId { get; set; }
    public Address Address { get; set; } = default!;

    public int MaxGuests { get; set; }
    public int Bedrooms { get; set; }
    public int Beds { get; set; }
    public int Bathrooms { get; set; }
    public decimal PricePerNight { get; set; }
    public TimeOnly CheckInTime { get; set; }
    public TimeOnly CheckOutTime { get; set; }

    public bool IsActive { get; set; }

    public int ReviewCount { get; set; }

    public double AverageRating { get; set; }

    public DateTime? LastBookedOnUtc { get; set; }

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();
    public ICollection<PropertyAmenity> PropertyAmenities { get; set; } = new List<PropertyAmenity>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
