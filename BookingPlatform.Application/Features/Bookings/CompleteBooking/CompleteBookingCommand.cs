using MediatR;
using BookingPlatform.Application.Common;
using System;

namespace BookingPlatform.Application.Features.Bookings.CompleteBooking;

public record CompleteBookingCommand(Guid BookingId) : IRequest<Result<bool>>;
