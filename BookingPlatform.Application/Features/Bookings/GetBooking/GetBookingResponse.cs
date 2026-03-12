using System;

namespace BookingPlatform.Application.Features.Bookings.GetBooking;

public record GetBookingResponse(
    Guid Id,
    Guid PropertyId,
    string PropertyName,
    string PropertyCity,
    string PropertyCountry,
    decimal PricePerNight,
    Guid GuestId,
    string GuestName,
    DateTime StartDate,
    DateTime EndDate,
    int GuestCount,
    decimal CleaningFee,
    decimal AmenitiesUpCharge,
    decimal PriceForPeriod,
    decimal TotalPrice,
    string BookingStatus,
    DateTime? CreatedOnUtc,
    DateTime? ConfirmedOnUtc,
    DateTime? RejectedOnUtc,
    DateTime? CompletedOnUtc,
    DateTime? CancelledOnUtc,
    bool HasReview
);
