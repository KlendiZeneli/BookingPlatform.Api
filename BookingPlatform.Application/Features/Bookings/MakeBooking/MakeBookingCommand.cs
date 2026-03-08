using MediatR;
using BookingPlatform.Application.Common;
using System;

namespace BookingPlatform.Application.Features.Bookings.MakeBooking;

public record MakeBookingCommand(Guid PropertyId, DateTime StartDate, DateTime EndDate, int GuestCount) : IRequest<Result<MakeBookingResponse>>;
