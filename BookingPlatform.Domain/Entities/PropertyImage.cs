using System;
using System.Collections.Generic;
using System.Text;

namespace BookingPlatform.Domain.Entities;

public class PropertyImage
{
    public Guid Id { get; set; }

    public Guid PropertyId { get; set; }
    public Property Property { get; set; } = default!;

    public string Url { get; set; } = default!;

    public bool IsPrimary { get; set; }
}