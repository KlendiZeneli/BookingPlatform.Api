using MediatR;
using BookingPlatform.Application.Common;
using System;

namespace BookingPlatform.Application.Features.Properties.GetAvailability;

public record GetAvailabilityQuery(Guid PropertyId, DateTime CheckIn, DateTime CheckOut) : IRequest<Result<GetAvailabilityResponse>>;
