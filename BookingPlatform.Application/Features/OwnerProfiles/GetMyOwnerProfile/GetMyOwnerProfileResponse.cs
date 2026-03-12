using System;

namespace BookingPlatform.Application.Features.OwnerProfiles.GetMyOwnerProfile;

public record GetMyOwnerProfileResponse(
    Guid UserId,
    string IdentityCardNumber,
    string VerificationStatus,
    DateTime? VerifiedAt,
    string? VerificationNotes,
    string? BusinessName,
    DateTime CreatedAt
);
