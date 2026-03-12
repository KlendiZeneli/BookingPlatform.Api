using BookingPlatform.Application.Common;
using BookingPlatform.Application.Common.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Features.Bookings.GetBooking;

public class GetBookingHandler : IRequestHandler<GetBookingQuery, Result<GetBookingResponse>>
{
    private readonly IBookingRepository _bookings;
    private readonly ICurrentUserService _currentUser;

    public GetBookingHandler(IBookingRepository bookings, ICurrentUserService currentUser)
    {
        _bookings = bookings;
        _currentUser = currentUser;
    }

    public async Task<Result<GetBookingResponse>> Handle(GetBookingQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId == null) return Errors.NotAuthenticated;

        var booking = await _bookings.GetByIdAsync(request.BookingId, ct);
        if (booking == null) return Errors.BookingNotFound;

        // only the guest or the property owner can view the booking
        var isGuest = booking.GuestId == userId.Value;
        var isOwner = booking.Property?.OwnerProfileId == userId.Value;
        if (!isGuest && !isOwner) return Errors.NotAuthorized;

        var guestName = booking.Guest != null
            ? $"{booking.Guest.FirstName} {booking.Guest.LastName}"
            : string.Empty;

        return new GetBookingResponse(
            booking.Id,
            booking.PropertyId,
            booking.Property?.Name ?? string.Empty,
            booking.Property?.Address?.City ?? string.Empty,
            booking.Property?.Address?.Country ?? string.Empty,
            booking.Property?.PricePerNight ?? 0,
            booking.GuestId,
            guestName,
            booking.StartDate,
            booking.EndDate,
            booking.GuestCount,
            booking.CleaningFee,
            booking.AmenitiesUpCharge,
            booking.PriceForPeriod,
            booking.TotalPrice,
            booking.BookingStatus.ToString(),
            booking.CreatedOnUtc,
            booking.ConfirmedOnUtc,
            booking.RejectedOnUtc,
            booking.CompletedOnUtc,
            booking.CancelledOnUtc,
            booking.Review != null
        );
    }
}
