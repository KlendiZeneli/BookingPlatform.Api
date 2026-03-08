using BookingPlatform.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingPlatform.Domain.Entities;

public class Amenity : BaseEntity
{
    public AmenitiesEnum Name { get; set; }
    public ICollection<PropertyAmenity> PropertyAmenities { get; set; } = new List<PropertyAmenity>();
}
