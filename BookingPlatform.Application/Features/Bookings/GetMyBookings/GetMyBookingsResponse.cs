using System;
using System.Collections.Generic;

namespace BookingPlatform.Application.Features.Bookings.GetMyBookings;

public record GetMyBookingsResponse(IEnumerable<MyBookingDto> Bookings);

public record MyBookingDto(
    Guid Id,
    Guid PropertyId,
    string PropertyName,
    string PropertyCity,
    string PropertyCountry,
    DateTime StartDate,
    DateTime EndDate,
    int GuestCount,
    decimal TotalPrice,
    string BookingStatus,
    DateTime? CreatedOnUtc,
    DateTime? ConfirmedOnUtc,
    DateTime? CancelledOnUtc,
    bool HasReview
);
