using BookingPlatform.Application.Common;
using BookingPlatform.Application.Common.Interfaces;
using BookingPlatform.Domain.Entities;
using BookingPlatform.Domain.Enums;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Features.OwnerProfiles.VerifyOwnerProfile;

public class VerifyOwnerProfileHandler : IRequestHandler<VerifyOwnerProfileCommand, Result>
{
    private readonly IOwnerProfileRepository _repo;
    private readonly IUserRepository _users;
    private readonly IRoleRepository _roles;

    public VerifyOwnerProfileHandler(IOwnerProfileRepository repo, IUserRepository users,IRoleRepository roles)
    {
        _repo = repo;
        _roles = roles;
        _users = users;
    }

    public async Task<Result> Handle(VerifyOwnerProfileCommand request, CancellationToken ct)
    {
        var profile = await _repo.GetByUserIdAsync(request.UserId, ct);
        if (profile == null)
            return Errors.OwnerProfileNotFound;
        var user = await _users.GetByUserIdAsync(request.UserId, ct);
        var ownerRole = await _roles.GetByNameAsync("Owner", ct);

        if (!user.UserRoles.Any(ur => ur.RoleId == ownerRole.Id))
        {
            user.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = ownerRole.Id,
                AssignedAt = DateTime.UtcNow
            });
        }

        profile.VerificationStatus = request.Approve ? VerificationStatusEnum.Verified : VerificationStatusEnum.Rejected;
        profile.VerifiedAt = request.Approve ? DateTime.UtcNow : null;
        profile.VerificationNotes = request.Notes;
        profile.LastModifiedAt = DateTime.UtcNow;

        await _users.SaveChangesAsync(ct);
        await _repo.SaveChangesAsync(ct);

        return Result.Success();
    }
}
