using BookingPlatform.Application.Common;
using BookingPlatform.Application.Common.Events;
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
    private readonly IEventProducer _events;

    public CancelBookingHandler(IBookingRepository bookings, ICurrentUserService currentUser, IEventProducer events)
    {
        _bookings = bookings;
        _currentUser = currentUser;
        _events = events;
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

        var oldStatus = booking.BookingStatus;
        booking.BookingStatus = BookingStatus.Cancelled;
        booking.CancelledOnUtc = System.DateTime.UtcNow;

        await _bookings.SaveChangesAsync(ct);

        await _events.ProduceAsync(KafkaTopics.BookingStatusChanged,
            new BookingStatusChangedEvent(
                booking.Id,
                booking.PropertyId,
                booking.GuestId,
                oldStatus.ToString(),
                booking.BookingStatus.ToString(),
                DateTime.UtcNow), ct);

        return Result.Success();
    }
}
