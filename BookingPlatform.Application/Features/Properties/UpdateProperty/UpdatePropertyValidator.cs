using FluentValidation;

namespace BookingPlatform.Application.Features.Properties.UpdateProperty;

public class UpdatePropertyValidator : AbstractValidator<UpdatePropertyCommand>
{
    public UpdatePropertyValidator()
    {
        RuleFor(x => x.PropertyId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.PropertyType).IsInEnum();
        RuleFor(x => x.MaxGuests).GreaterThan(0);
        RuleFor(x => x.Bedrooms).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Beds).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Bathrooms).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PricePerNight).GreaterThan(0);
        RuleFor(x => x.CheckOutTime).NotEqual(x => x.CheckInTime)
            .WithMessage("Check-out time must differ from check-in time.");
        RuleFor(x => x.AmenityNames).NotNull();
        RuleForEach(x => x.AmenityNames)
            .Must(name => Enum.TryParse<BookingPlatform.Domain.Enums.AmenitiesEnum>(name, true, out _))
            .WithMessage("Invalid amenity name: {PropertyValue}");
    }
}
