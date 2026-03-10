using FluentValidation;
using BookingPlatform.Application.Common.Validators;

namespace BookingPlatform.Application.Features.Auth.ResetPassword;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token is required.");

        RuleFor(x => x.NewPassword)
            .MustBeValidPassword();
    }
}