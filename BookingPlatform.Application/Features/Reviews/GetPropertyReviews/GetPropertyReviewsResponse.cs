using System;
using System.Collections.Generic;

namespace BookingPlatform.Application.Features.Reviews.GetPropertyReviews;

public record GetPropertyReviewsResponse(IEnumerable<ReviewDto> Reviews);

public record ReviewDto(
    Guid Id,
    Guid BookingId,
    Guid GuestId,
    string GuestName,
    int Rating,
    string? Comment
);
