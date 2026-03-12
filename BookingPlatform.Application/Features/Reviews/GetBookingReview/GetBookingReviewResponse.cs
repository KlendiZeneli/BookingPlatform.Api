using System;

namespace BookingPlatform.Application.Features.Reviews.GetBookingReview;

public record GetBookingReviewResponse(
    Guid? ReviewId,
    bool HasReview,
    int? Rating,
    string? Comment
);
