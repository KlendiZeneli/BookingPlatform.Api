using BookingPlatform.Application.Common;
using BookingPlatform.Application.Common.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace BookingPlatform.Application.Features.Reviews.GetPropertyReviews;

public class GetPropertyReviewsHandler : IRequestHandler<GetPropertyReviewsQuery, Result<GetPropertyReviewsResponse>>
{
    private readonly IReviewRepository _reviews;

    public GetPropertyReviewsHandler(IReviewRepository reviews)
    {
        _reviews = reviews;
    }

    public async Task<Result<GetPropertyReviewsResponse>> Handle(GetPropertyReviewsQuery request, CancellationToken ct)
    {
        var items = await _reviews.GetByPropertyIdAsync(request.PropertyId, ct);

        var dtos = items.Select(r => new ReviewDto(
            r.Id,
            r.BookingId,
            r.GuestId,
            r.Guest != null ? r.Guest.FirstName + " " + r.Guest.LastName : string.Empty,
            r.Rating,
            r.Comment
        ));

        return new GetPropertyReviewsResponse(dtos);
    }
}
