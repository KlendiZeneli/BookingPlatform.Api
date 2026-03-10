using MediatR;
using BookingPlatform.Application.Common;

namespace BookingPlatform.Application.Features.Auth.ResetPassword;

public record ResetPasswordCommand(string Token, string NewPassword) : IRequest<Result>;