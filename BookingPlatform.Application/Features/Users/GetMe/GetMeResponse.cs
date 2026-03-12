using System;
using System.Collections.Generic;

namespace BookingPlatform.Application.Features.Users.GetMe;

public record GetMeResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    string? ProfileImageUrl,
    bool IsActive,
    IEnumerable<string> Roles
);
