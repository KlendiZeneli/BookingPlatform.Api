using BookingPlatform.Application.Common;
using MediatR;
using System;

namespace BookingPlatform.Application.Features.Reviews.GetPropertyReviews;

public record GetPropertyReviewsQuery(Guid PropertyId) : IRequest<Result<GetPropertyReviewsResponse>>;
