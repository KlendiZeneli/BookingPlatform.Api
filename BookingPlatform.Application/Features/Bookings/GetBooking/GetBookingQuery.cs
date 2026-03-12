using MediatR;
using BookingPlatform.Application.Common;
using System;

namespace BookingPlatform.Application.Features.Bookings.GetBooking;

public record GetBookingQuery(Guid BookingId) : IRequest<Result<GetBookingResponse>>;
