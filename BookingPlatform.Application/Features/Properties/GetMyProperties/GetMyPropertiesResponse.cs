using System;
using System.Collections.Generic;

namespace BookingPlatform.Application.Features.Properties.GetMyProperties;

public record GetMyPropertiesResponse(IEnumerable<MyPropertyDto> Properties);

public record MyPropertyDto(
    Guid Id,
    string Name,
    string Description,
    string PropertyType,
    string City,
    string Country,
    decimal PricePerNight,
    int MaxGuests,
    int Bedrooms,
    int Beds,
    int Bathrooms,
    double AverageRating,
    int ReviewCount,
    bool IsActive,
    int PendingBookingsCount,
    int ConfirmedBookingsCount
);
