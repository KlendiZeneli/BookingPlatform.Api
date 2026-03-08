using MediatR;
using BookingPlatform.Application.Common;
using System;

namespace BookingPlatform.Application.Features.OwnerProfiles.VerifyOwnerProfile;

public record VerifyOwnerProfileCommand(Guid UserId, bool Approve, string? Notes) : IRequest<Result>;
