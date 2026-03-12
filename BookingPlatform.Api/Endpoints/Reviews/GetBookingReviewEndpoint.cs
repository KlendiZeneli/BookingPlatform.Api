using BookingPlatform.Api.Endpoints;
using BookingPlatform.Application.Features.Reviews.GetBookingReview;
using MediatR;
using System;

namespace BookingPlatform.Api.Endpoints.Reviews;

public class GetBookingReviewEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/bookings/{bookingId:guid}/review", async (Guid bookingId, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetBookingReviewQuery(bookingId), ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Json(result.Error!.Description, statusCode: result.Error.Code);
        })
        .RequireAuthorization()
        .WithTags("Reviews")
        .WithSummary("Check whether the current user has reviewed a booking");
    }
}
