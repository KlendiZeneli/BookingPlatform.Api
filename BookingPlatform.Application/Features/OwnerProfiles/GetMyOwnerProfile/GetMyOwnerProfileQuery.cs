using MediatR;
using BookingPlatform.Application.Common;

namespace BookingPlatform.Application.Features.OwnerProfiles.GetMyOwnerProfile;

public record GetMyOwnerProfileQuery() : IRequest<Result<GetMyOwnerProfileResponse>>;
