using FluentValidation;
using System;

namespace BookingPlatform.Application.Features.OwnerProfiles.CreateOwnerProfile;

public class CreateOwnerProfileValidator : AbstractValidator<CreateOwnerProfileCommand>
{
    public CreateOwnerProfileValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required");

        RuleFor(x => x.IdentityCardNumber)
            .NotEmpty().WithMessage("Identity card number is required").MinimumLength(10).WithMessage("ID number needs to be 10 or more characters.");

        RuleFor(x => x.CreditCard)
            .NotEmpty().WithMessage("Credit card is required").Length(12).WithMessage("Credit Card number needs to be 12 characters long.");
    }
}
