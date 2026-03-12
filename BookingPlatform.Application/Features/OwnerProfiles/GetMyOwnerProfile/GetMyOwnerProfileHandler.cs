using BookingPlatform.Application.Common;
using BookingPlatform.Application.Common.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Features.OwnerProfiles.GetMyOwnerProfile;

public class GetMyOwnerProfileHandler : IRequestHandler<GetMyOwnerProfileQuery, Result<GetMyOwnerProfileResponse>>
{
    private readonly IOwnerProfileRepository _ownerProfiles;
    private readonly ICurrentUserService _currentUser;

    public GetMyOwnerProfileHandler(IOwnerProfileRepository ownerProfiles, ICurrentUserService currentUser)
    {
        _ownerProfiles = ownerProfiles;
        _currentUser = currentUser;
    }

    public async Task<Result<GetMyOwnerProfileResponse>> Handle(GetMyOwnerProfileQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId == null) return Errors.NotAuthenticated;

        var profile = await _ownerProfiles.GetByUserIdAsync(userId.Value, ct);
        if (profile == null) return Errors.OwnerProfileNotFound;

        return new GetMyOwnerProfileResponse(
            profile.UserId,
            profile.IdentityCardNumber,
            profile.VerificationStatus.ToString(),
            profile.VerifiedAt,
            profile.VerificationNotes,
            profile.BusinessName,
            profile.CreatedAt
        );
    }
}
