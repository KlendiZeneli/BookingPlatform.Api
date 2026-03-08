using FluentValidation;

namespace BookingPlatform.Application.Features.Reviews.MakeReview;

public class MakeReviewValidator : AbstractValidator<MakeReviewCommand>
{
    public MakeReviewValidator()
    {
        RuleFor(x => x.BookingId).NotEmpty();
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
    }
}
