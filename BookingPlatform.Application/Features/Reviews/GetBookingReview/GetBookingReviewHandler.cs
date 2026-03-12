using BookingPlatform.Application.Common;
using BookingPlatform.Application.Common.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Features.Reviews.GetBookingReview;

public class GetBookingReviewHandler : IRequestHandler<GetBookingReviewQuery, Result<GetBookingReviewResponse>>
{
    private readonly IReviewRepository _reviews;
    private readonly ICurrentUserService _currentUser;

    public GetBookingReviewHandler(IReviewRepository reviews, ICurrentUserService currentUser)
    {
        _reviews = reviews;
        _currentUser = currentUser;
    }

    public async Task<Result<GetBookingReviewResponse>> Handle(GetBookingReviewQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId == null) return Errors.NotAuthenticated;

        var review = await _reviews.GetByBookingAndGuestAsync(request.BookingId, userId.Value, ct);

        return review == null
            ? new GetBookingReviewResponse(null, false, null, null)
            : new GetBookingReviewResponse(review.Id, true, review.Rating, review.Comment);
    }
}
