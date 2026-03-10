using FluentValidation;
using BookingPlatform.Application.Common.Validators;

namespace BookingPlatform.Application.Features.Auth.ChangePassword;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required.");

        RuleFor(x => x.NewPassword)
            .MustBeValidPassword();
    }
}