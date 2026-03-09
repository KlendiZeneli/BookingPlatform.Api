using BookingPlatform.Application.Common;
using BookingPlatform.Application.Common.Interfaces;
using BookingPlatform.Domain.Enums;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Features.Bookings.VerifyBooking;

public class VerifyBookingHandler : IRequestHandler<VerifyBookingCommand, Result<VerifyBookingResponse>>
{
    private readonly IBookingRepository _bookings;
    private readonly IPropertyRepository _properties;
    private readonly ICurrentUserService _currentUser;
    private readonly IEmailService _emails;

    public VerifyBookingHandler(IBookingRepository bookings, IPropertyRepository properties, ICurrentUserService currentUser, IEmailService emails)
    {
        _bookings = bookings;
        _properties = properties;
        _currentUser = currentUser;
        _emails = emails;
    }

    public async Task<Result<VerifyBookingResponse>> Handle(VerifyBookingCommand request, CancellationToken ct)
    {
        var booking = await _bookings.GetByIdAsync(request.BookingId, ct);
        if (booking == null) return Errors.BookingNotFound;

        var property = await _properties.GetByIdAsync(booking.PropertyId, ct);
        if (property == null) return Errors.PropertyNotFound;

        var currentUserId = _currentUser.UserId;
        if (currentUserId == null) return Errors.NotAuthenticated;

        // ensure current user is the owner of the property
        if (property.OwnerProfileId != currentUserId.Value) return Errors.NotAuthorized;

        booking.BookingStatus = BookingStatus.Confirmed;
        booking.ConfirmedOnUtc = DateTime.UtcNow;

        await _bookings.SaveChangesAsync(ct);

        // send a simple notification email to guest (placeholder)
        var guest = booking.Guest;
        if (guest != null)
        {
            await _emails.SendEmailAsync(guest.Email, "Your booking is confirmed", $"Your booking {booking.Id} was confirmed by the owner.", ct);
        }

        return new VerifyBookingResponse(true);
    }
}
