using MediatR;
using BookingPlatform.Application.Common;

namespace BookingPlatform.Application.Features.Users.GetMe;

public record GetMeQuery() : IRequest<Result<GetMeResponse>>;
