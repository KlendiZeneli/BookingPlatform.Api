using MediatR;
using System;
using BookingPlatform.Application.Common;

namespace BookingPlatform.Application.Features.Bookings.VerifyBooking;

public record VerifyBookingCommand(Guid BookingId) : IRequest<Result<VerifyBookingResponse>>;
