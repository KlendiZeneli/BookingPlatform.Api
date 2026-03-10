using MediatR;
using BookingPlatform.Application.Common;

namespace BookingPlatform.Application.Features.Auth.ChangePassword;

public record ChangePasswordCommand(string CurrentPassword, string NewPassword) : IRequest<Result>;
