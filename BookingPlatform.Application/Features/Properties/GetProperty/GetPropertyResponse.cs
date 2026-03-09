using System;
using System.Collections.Generic;

namespace BookingPlatform.Application.Features.Properties.GetProperty;

public record GetPropertyResponse(PropertyDto Property);

public record PropertyDto(
    Guid Id,
    Guid OwnerProfileId,
    string Name,
    string Description,
    int PropertyType,
    Guid AddressId,
    AddressDto? Address,
    int MaxGuests,
    int Bedrooms,
    int Beds,
    int Bathrooms,
    decimal PricePerNight,
    string CheckInTime,
    string CheckOutTime,
    bool IsActive,
    int ReviewCount,
    double AverageRating,
    DateTime? LastBookedOnUtc,
    IEnumerable<BookingDto> Bookings
);

public record BookingDto(
    Guid Id,
    Guid PropertyId,
    Guid GuestId,
    DateTime StartDate,
    DateTime EndDate,
    int GuestCount,
    decimal TotalPrice,
    string BookingStatus,
    DateTime? CreatedOnUtc,
    DateTime? ConfirmedOnUtc,
    DateTime? RejectedOnUtc,
    DateTime? CompletedOnUtc,
    DateTime? CancelledOnUtc
);

public record AddressDto(string Country, string City, string Street, string PostalCode);
