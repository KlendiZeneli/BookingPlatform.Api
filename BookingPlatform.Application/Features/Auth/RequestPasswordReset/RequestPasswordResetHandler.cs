using BookingPlatform.Application.Common;
using BookingPlatform.Application.Common.Events;
using BookingPlatform.Application.Common.Interfaces;
using MediatR;
using System.Security.Cryptography;

namespace BookingPlatform.Application.Features.Auth.RequestPasswordReset;

public class RequestPasswordResetHandler : IRequestHandler<RequestPasswordResetCommand, Result>
{
    private readonly IUserRepository _users;
    private readonly IEventProducer _events;

    public RequestPasswordResetHandler(IUserRepository users, IEventProducer events)
    {
        _users = users;
        _events = events;
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

        await _events.ProduceAsync(KafkaTopics.PasswordResetRequested,
            new PasswordResetRequestedEvent(
                user.Id,
                user.Email,
                user.FirstName,
                token,
                user.PasswordResetExpires!.Value,
                resetUrl,
                DateTime.UtcNow), ct);

        return Result.Success();
    }
}