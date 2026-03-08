using FluentValidation;

namespace BookingPlatform.Application.Features.Bookings.MakeBooking;

public class MakeBookingValidator : AbstractValidator<MakeBookingCommand>
{
    public MakeBookingValidator()
    {
        RuleFor(x => x.PropertyId).NotEmpty();
        RuleFor(x => x.StartDate).LessThan(x => x.EndDate).WithMessage("StartDate must be before EndDate");
        RuleFor(x => x.GuestCount).GreaterThan(0);
    }
}
