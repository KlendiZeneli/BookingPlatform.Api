using MediatR;
using BookingPlatform.Application.Common;
using System;

namespace BookingPlatform.Application.Features.OwnerProfiles.CreateOwnerProfile;

public record CreateOwnerProfileCommand(Guid UserId, string IdentityCardNumber, string? BusinessName, string CreditCard) : IRequest<Result<CreateOwnerProfileResponse>>;
