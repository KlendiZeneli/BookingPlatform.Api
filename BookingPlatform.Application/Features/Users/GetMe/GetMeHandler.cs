using BookingPlatform.Application.Common;
using BookingPlatform.Application.Common.Interfaces;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Features.Users.GetMe;

public class GetMeHandler : IRequestHandler<GetMeQuery, Result<GetMeResponse>>
{
    private readonly IUserRepository _users;
    private readonly ICurrentUserService _currentUser;

    public GetMeHandler(IUserRepository users, ICurrentUserService currentUser)
    {
        _users = users;
        _currentUser = currentUser;
    }

    public async Task<Result<GetMeResponse>> Handle(GetMeQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId == null) return Errors.NotAuthenticated;

        var user = await _users.GetByIdWithRolesAsync(userId.Value, ct);
        if (user == null) return Errors.UserNotFound;

        var roles = user.UserRoles.Select(ur => ur.Role.Name);

        return new GetMeResponse(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.PhoneNumber,
            user.ProfileImageUrl,
            user.IsActive,
            roles
        );
    }
}
