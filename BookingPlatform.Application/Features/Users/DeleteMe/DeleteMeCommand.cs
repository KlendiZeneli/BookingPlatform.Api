using MediatR;
using BookingPlatform.Application.Common;

namespace BookingPlatform.Application.Features.Users.DeleteMe;

public record DeleteMeCommand() : IRequest<Result<bool>>;
