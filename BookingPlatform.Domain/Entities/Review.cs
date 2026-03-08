using System;
using System.Collections.Generic;
using System.Text;

namespace BookingPlatform.Domain.Entities;

public class Review : BaseEntity
{
    public Guid BookingId { get; set; }
    public Booking Booking { get; set; } = default!;

    public Guid GuestId { get; set; }
    public User Guest { get; set; } = default!;

    public Guid PropertyId { get; set; }
    public Property Property { get; set; } = default!;

    public int Rating { get; set; }
    public string? Comment { get; set; }
}

