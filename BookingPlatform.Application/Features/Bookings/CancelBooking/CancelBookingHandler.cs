using BookingPlatform.Application.Common;
using BookingPlatform.Application.Common.Interfaces;
using BookingPlatform.Domain.Enums;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Features.Bookings.CancelBooking;

public class CancelBookingHandler : IRequestHandler<CancelBookingCommand, Result>
{
    private readonly IBookingRepository _bookings;
    private readonly ICurrentUserService _currentUser;

    public CancelBookingHandler(IBookingRepository bookings, ICurrentUserService currentUser)
    {
        _bookings = bookings;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(CancelBookingCommand request, CancellationToken ct)
    {
        var booking = await _bookings.GetByIdAsync(request.BookingId, ct);
        if (booking == null) return Errors.BookingNotFound;

        var currentUserId = _currentUser.UserId;
        if (currentUserId == null) return Errors.NotAuthenticated;

        if (booking.GuestId != currentUserId.Value) return Errors.NotAuthorized;

        if (booking.BookingStatus == BookingStatus.Confirmed)
            return Errors.NotAuthorized; // or create a dedicated error

        booking.BookingStatus = BookingStatus.Cancelled;
        booking.CancelledOnUtc = System.DateTime.UtcNow;

        await _bookings.SaveChangesAsync(ct);

        return Result.Success();
    }
}
