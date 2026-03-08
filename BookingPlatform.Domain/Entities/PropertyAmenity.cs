using System;
using System.Collections.Generic;
using System.Text;

namespace BookingPlatform.Domain.Entities;

public class PropertyAmenity
{
    public Guid PropertyId { get; set; }
    public Property Property { get; set; } = default!;

    public Guid AmenityId { get; set; }
    public Amenity Amenity { get; set; } = default!;
}
