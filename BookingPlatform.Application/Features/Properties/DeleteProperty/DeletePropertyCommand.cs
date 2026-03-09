using BookingPlatform.Application.Common;
using MediatR;
using System;

namespace BookingPlatform.Application.Features.Properties.DeleteProperty;

public record DeletePropertyCommand(Guid PropertyId) : IRequest<Result>;
