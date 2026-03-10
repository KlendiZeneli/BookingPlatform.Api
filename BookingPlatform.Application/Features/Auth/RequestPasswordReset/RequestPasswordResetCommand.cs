using MediatR;
using BookingPlatform.Application.Common;

namespace BookingPlatform.Application.Features.Auth.RequestPasswordReset;

public record RequestPasswordResetCommand(string Email) : IRequest<Result>;
