using MediatR;
using BookingPlatform.Application.Common;

namespace BookingPlatform.Application.Features.Bookings.GetMyBookings;

public record GetMyBookingsQuery() : IRequest<Result<GetMyBookingsResponse>>;
