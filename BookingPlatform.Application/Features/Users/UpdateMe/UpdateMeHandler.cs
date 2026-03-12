using BookingPlatform.Application.Common;
using BookingPlatform.Application.Common.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Features.Users.UpdateMe;

public class UpdateMeHandler : IRequestHandler<UpdateMeCommand, Result<bool>>
{
    private readonly IUserRepository _users;
    private readonly ICurrentUserService _currentUser;

    public UpdateMeHandler(IUserRepository users, ICurrentUserService currentUser)
    {
        _users = users;
        _currentUser = currentUser;
    }

    public async Task<Result<bool>> Handle(UpdateMeCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId == null) return Errors.NotAuthenticated;

        var user = await _users.GetByIdAsync(userId.Value, ct);
        if (user == null) return Errors.UserNotFound;

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.PhoneNumber;
        user.LastModifiedAt = DateTime.UtcNow;

        await _users.UpdateAsync(user, ct);
        await _users.SaveChangesAsync(ct);

        return true;
    }
}
