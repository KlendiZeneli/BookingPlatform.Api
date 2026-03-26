using BookingPlatform.Application.Common;
using BookingPlatform.Application.Common.Events;
using BookingPlatform.Application.Common.Interfaces;
using BookingPlatform.Domain.Enums;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Features.Bookings.CompleteBooking;

public class CompleteBookingHandler : IRequestHandler<CompleteBookingCommand, Result<bool>>
{
    private readonly IBookingRepository _bookings;
    private readonly ICurrentUserService _currentUser;
    private readonly IEventProducer _events;

    public CompleteBookingHandler(IBookingRepository bookings, ICurrentUserService currentUser, IEventProducer events)
    {
        _bookings = bookings;
        _currentUser = currentUser;
        _events = events;
    }

    public async Task<Result<bool>> Handle(CompleteBookingCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId == null) return Errors.NotAuthenticated;

        var booking = await _bookings.GetByIdAsync(request.BookingId, ct);
        if (booking == null) return Errors.BookingNotFound;

        // only the property owner can mark as completed
        if (booking.Property?.OwnerProfileId != userId.Value) return Errors.NotAuthorized;

        if (booking.BookingStatus != BookingStatus.Confirmed)
            return Errors.BookingNotConfirmed;

        var oldStatus = booking.BookingStatus;
        booking.BookingStatus = BookingStatus.Completed;
        booking.CompletedOnUtc = DateTime.UtcNow;

        await _bookings.SaveChangesAsync(ct);

        await _events.ProduceAsync(KafkaTopics.BookingStatusChanged,
            new BookingStatusChangedEvent(
                booking.Id,
                booking.PropertyId,
                booking.GuestId,
                oldStatus.ToString(),
                booking.BookingStatus.ToString(),
                DateTime.UtcNow), ct);

        return true;
    }
}
