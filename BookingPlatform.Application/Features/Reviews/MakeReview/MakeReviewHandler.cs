using BookingPlatform.Application.Common;
using BookingPlatform.Application.Common.Interfaces;
using BookingPlatform.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Features.Reviews.MakeReview;

public class MakeReviewHandler : IRequestHandler<MakeReviewCommand, Result<MakeReviewResponse>>
{
    private readonly IReviewRepository _reviews;
    private readonly IBookingRepository _bookings;
    private readonly IPropertyRepository _properties;
    private readonly ICurrentUserService _currentUser;

    public MakeReviewHandler(IReviewRepository reviews, IBookingRepository bookings, IPropertyRepository properties, ICurrentUserService currentUser)
    {
        _reviews = reviews;
        _bookings = bookings;
        _properties = properties;
        _currentUser = currentUser;
    }

    public async Task<Result<MakeReviewResponse>> Handle(MakeReviewCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId == null) return Errors.NotAuthenticated;

        var existing = await _reviews.GetByBookingAndGuestAsync(request.BookingId, userId.Value, ct);
        if (existing != null) return Errors.AlreadyReviewed;

        var booking = await _bookings.GetByIdAsync(request.BookingId, ct);
        if (booking == null) return Errors.BookingNotFound;
        if (booking.GuestId != userId.Value) return Errors.NotAuthenticated;
        if (booking.BookingStatus != BookingPlatform.Domain.Enums.BookingStatus.Confirmed) return Errors.ReviewNotAllowed;

        var review = new Review
        {
            Id = Guid.NewGuid(),
            BookingId = request.BookingId,
            GuestId = userId.Value,
            PropertyId = booking.PropertyId,
            Rating = request.Rating,
            Comment = request.Comment,
            CreatedAt = DateTime.UtcNow
        };

        await _reviews.AddAsync(review, ct);

        var property = await _properties.GetByIdAsync(booking.PropertyId, ct);
        if (property != null)
        {
            var currentCount = property.ReviewCount;
            var currentAvg = property.AverageRating;
            var newCount = currentCount + 1;
            var newAvg = ((currentAvg * currentCount) + review.Rating) / newCount;

            property.ReviewCount = newCount;
            property.AverageRating = newAvg;
        }

        await _reviews.SaveChangesAsync(ct);

        return new MakeReviewResponse(review.Id);
    }
}
