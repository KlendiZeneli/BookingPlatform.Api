using BookingPlatform.Application.Common;
using BookingPlatform.Application.Common.Interfaces;
using MediatR;

namespace BookingPlatform.Application.Features.Auth.ResetPassword;

public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IUserRepository _users;

    public ResetPasswordHandler(IUserRepository users)
    {
        _users = users;
    }

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken ct)
    {
        var user = await _users.GetByResetTokenAsync(request.Token, ct);

        if (user == null || !user.PasswordResetExpires.HasValue || user.PasswordResetExpires.Value < DateTime.UtcNow)
            return Errors.InvalidOrExpiredToken;

        user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.PasswordResetToken = null;
        user.PasswordResetExpires = null;

        await _users.UpdateAsync(user, ct);
        await _users.SaveChangesAsync(ct);

        return Result.Success();
    }
}