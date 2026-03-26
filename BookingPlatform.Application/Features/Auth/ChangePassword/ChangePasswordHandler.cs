using BookingPlatform.Application.Common;
using BookingPlatform.Application.Common.Events;
using BookingPlatform.Application.Common.Interfaces;
using MediatR;
using System.Text.RegularExpressions;

namespace BookingPlatform.Application.Features.Auth.ChangePassword;

public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly IUserRepository _users;
    private readonly ICurrentUserService _currentUser;
    private readonly IEventProducer _events;

    public ChangePasswordHandler(IUserRepository users, ICurrentUserService currentUser, IEventProducer events)
    {
        _users = users;
        _currentUser = currentUser;
        _events = events;
    }

    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            return Errors.NotAuthenticated;

        var user = await _users.GetByUserIdAsync(userId.Value, ct);
        if (user == null)
            return Errors.UserNotFound;

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.Password))
            return Errors.WrongPassword;

        if (!IsValidPassword(request.NewPassword))
            return Errors.PasswordFormat;

        user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

        await _users.UpdateAsync(user, ct);
        await _users.SaveChangesAsync(ct);

        await _events.ProduceAsync(KafkaTopics.PasswordChanged,
            new PasswordChangedEvent(user.Id, user.Email, DateTime.UtcNow), ct);

        return Result.Success();
    }

    private static bool IsValidPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            return false;

        return Regex.IsMatch(password, "[A-Z]")
            && Regex.IsMatch(password, "[a-z]")
            && Regex.IsMatch(password, "[0-9]")
            && Regex.IsMatch(password, "[^a-zA-Z0-9]");
    }
}