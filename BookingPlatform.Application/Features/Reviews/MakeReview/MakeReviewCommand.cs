using MediatR;
using BookingPlatform.Application.Common;
using System;

namespace BookingPlatform.Application.Features.Reviews.MakeReview;

public record MakeReviewCommand(Guid BookingId, int Rating, string? Comment) : IRequest<Result<MakeReviewResponse>>;
