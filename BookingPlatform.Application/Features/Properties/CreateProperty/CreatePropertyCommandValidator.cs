using FluentValidation;

namespace BookingPlatform.Application.Features.Properties.CreateProperty;

public class CreatePropertyValidator : AbstractValidator<CreatePropertyCommand>
{
    public CreatePropertyValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(2000);

        RuleFor(x => x.PropertyType).IsInEnum().WithMessage("'Property Type' must be a valid value."); ;

        // AddressId removed: address provided inline via Address DTO

        RuleFor(x => x.MaxGuests)
            .GreaterThan(0);

        RuleFor(x => x.Bedrooms)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Beds)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Bathrooms)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.PricePerNight)
            .GreaterThan(0);

        RuleFor(x => x.CheckOutTime)
            .NotEqual(x => x.CheckInTime)
            .WithMessage("Check-out time must differ from check-in time.");

        RuleFor(x => x.Amenities)
            .NotNull();

        RuleForEach(x => x.Amenities)
            .Must(name => Enum.TryParse<BookingPlatform.Domain.Enums.AmenitiesEnum>(name, true, out _))
            .WithMessage("Invalid amenity name: {PropertyValue}");

        RuleFor(x => x.Address).NotNull();
        RuleFor(x => x.Address.Country).NotEmpty().WithMessage("Country is required");
        RuleFor(x => x.Address.City).NotEmpty().WithMessage("City is required");
        RuleFor(x => x.Address.Street).NotEmpty().WithMessage("Street is required");
        RuleFor(x => x.Address.PostalCode).NotEmpty().WithMessage("Postal code is required");
    }
}