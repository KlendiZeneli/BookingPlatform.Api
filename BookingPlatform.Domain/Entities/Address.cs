using System;
using System.Collections.Generic;
using System.Text;

namespace BookingPlatform.Domain.Entities;

public class Address : BaseEntity
{
    public string Country { get; set; } = default!;
    public string City { get; set; } = default!;
    public string Street { get; set; } = default!;
    public string PostalCode { get; set; } = default!;

    public ICollection<Property> Properties { get; set; } = new List<Property>();
}
