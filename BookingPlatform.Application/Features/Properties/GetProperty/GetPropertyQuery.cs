using BookingPlatform.Application.Common;
using MediatR;
using System;

namespace BookingPlatform.Application.Features.Properties.GetProperty;

public record GetPropertyQuery(Guid PropertyId) : IRequest<Result<GetPropertyResponse>>;
