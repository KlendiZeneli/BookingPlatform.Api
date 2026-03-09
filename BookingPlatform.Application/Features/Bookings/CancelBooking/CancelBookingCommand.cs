using BookingPlatform.Application.Common;
using MediatR;
using System;

namespace BookingPlatform.Application.Features.Bookings.CancelBooking;

public record CancelBookingCommand(Guid BookingId) : IRequest<Result>;
