using System;
using System.Collections.Generic;
using System.Text;

namespace BookingPlatform.Domain.Entities;

public class OwnerProfile
{
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;

    public string IdentityCardNumber { get; set; } = default!;
    public string VerificationStatus { get; set; } = default!;
    public string? BusinessName { get; set; }
    public string CreditCard { get; set; } = default!;

    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
}

