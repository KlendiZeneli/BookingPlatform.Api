using FluentValidation;

namespace BookingPlatform.Application.Features.Properties.SearchProperties;

public class SearchPropertiesCommandValidator : AbstractValidator<SearchPropertiesCommand>
{
    public SearchPropertiesCommandValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50).WithMessage("Page size must be between 1 and 50.");

        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum price cannot be negative.")
            .When(x => x.MinPrice.HasValue);

        RuleFor(x => x.MaxPrice)
            .GreaterThan(x => x.MinPrice ?? 0).WithMessage("Maximum price must be greater than minimum price.")
            .When(x => x.MaxPrice.HasValue);

        RuleFor(x => x.CheckOut)
            .GreaterThan(x => x.CheckIn).WithMessage("Check-out must be after check-in.")
            .When(x => x.CheckIn.HasValue && x.CheckOut.HasValue);

        RuleFor(x => x.Guests)
            .GreaterThan(0).WithMessage("Guest count must be at least 1.")
            .When(x => x.Guests.HasValue);

        RuleFor(x => x.MinRating)
            .InclusiveBetween(0, 5).WithMessage("Rating must be between 0 and 5.")
            .When(x => x.MinRating.HasValue);
    }
}