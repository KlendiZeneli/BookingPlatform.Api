using BookingPlatform.Application.Common;
using BookingPlatform.Application.Common.Interfaces;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Features.Bookings.GetMyBookings;

public class GetMyBookingsHandler : IRequestHandler<GetMyBookingsQuery, Result<GetMyBookingsResponse>>
{
    private readonly IBookingRepository _bookings;
    private readonly ICurrentUserService _currentUser;

    public GetMyBookingsHandler(IBookingRepository bookings, ICurrentUserService currentUser)
    {
        _bookings = bookings;
        _currentUser = currentUser;
    }

    public async Task<Result<GetMyBookingsResponse>> Handle(GetMyBookingsQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId == null) return Errors.NotAuthenticated;

        var bookings = await _bookings.GetByGuestIdAsync(userId.Value, ct);

        var dtos = bookings.Select(b => new MyBookingDto(
            b.Id,
            b.PropertyId,
            b.Property?.Name ?? string.Empty,
            b.Property?.Address?.City ?? string.Empty,
            b.Property?.Address?.Country ?? string.Empty,
            b.StartDate,
            b.EndDate,
            b.GuestCount,
            b.TotalPrice,
            b.BookingStatus.ToString(),
            b.CreatedOnUtc,
            b.ConfirmedOnUtc,
            b.CancelledOnUtc,
            b.Review != null
        ));

        return new GetMyBookingsResponse(dtos);
    }
}
