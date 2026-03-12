using MediatR;
using BookingPlatform.Application.Common;
using System;

namespace BookingPlatform.Application.Features.Reviews.GetBookingReview;

public record GetBookingReviewQuery(Guid BookingId) : IRequest<Result<GetBookingReviewResponse>>;
