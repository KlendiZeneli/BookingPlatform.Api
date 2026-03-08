using FluentValidation;
using System;

namespace BookingPlatform.Application.Features.OwnerProfiles.VerifyOwnerProfile;

public class VerifyOwnerProfileValidator : AbstractValidator<VerifyOwnerProfileCommand>
{
    public VerifyOwnerProfileValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required");
    }
}
