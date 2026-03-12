using BookingPlatform.Application.Common;
using BookingPlatform.Application.Common.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Features.Users.DeleteMe;

public class DeleteMeHandler : IRequestHandler<DeleteMeCommand, Result<bool>>
{
    private readonly IUserRepository _users;
    private readonly ICurrentUserService _currentUser;

    public DeleteMeHandler(IUserRepository users, ICurrentUserService currentUser)
    {
        _users = users;
        _currentUser = currentUser;
    }

    public async Task<Result<bool>> Handle(DeleteMeCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId == null) return Errors.NotAuthenticated;

        var user = await _users.GetByIdAsync(userId.Value, ct);
        if (user == null) return Errors.UserNotFound;

        // soft-delete: deactivate account rather than hard-delete so FK constraints are preserved
        user.IsActive = false;
        user.LastModifiedAt = DateTime.UtcNow;

        await _users.UpdateAsync(user, ct);
        await _users.SaveChangesAsync(ct);

        return true;
    }
}
