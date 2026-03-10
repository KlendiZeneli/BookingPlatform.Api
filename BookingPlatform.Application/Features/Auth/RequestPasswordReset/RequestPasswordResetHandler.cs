using BookingPlatform.Application.Common;
using BookingPlatform.Application.Common.Interfaces;
using MediatR;
using System.Security.Cryptography;

namespace BookingPlatform.Application.Features.Auth.RequestPasswordReset;

public class RequestPasswordResetHandler : IRequestHandler<RequestPasswordResetCommand, Result>
{
    private readonly IUserRepository _users;
    private readonly IEmailService _emails;

    public RequestPasswordResetHandler(IUserRepository users, IEmailService emails)
    {
        _users = users;
        _emails = emails;
    }

    public async Task<Result> Handle(RequestPasswordResetCommand request, CancellationToken ct)
    {
        var user = await _users.GetByEmailAsync(request.Email, ct);

        // Always return success - never reveal whether the email exists
        if (user == null)
            return Result.Success();

        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        user.PasswordResetToken = token;
        user.PasswordResetExpires = DateTime.UtcNow.AddHours(1);

        await _users.UpdateAsync(user, ct);
        await _users.SaveChangesAsync(ct);

        var resetUrl = "http://localhost:4200/reset-password?token=" + token;
        var html = "<p>Hello " + user.FirstName + ",</p><p>To reset your password click the link below (valid for 1 hour):</p><p><a href=" + resetUrl + ">Reset password</a></p>";

        await _emails.SendEmailAsync(user.Email, "Reset your password", html);

        return Result.Success();
    }
}