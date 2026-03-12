using BookingPlatform.Api.Endpoints;
using BookingPlatform.Application.Features.Bookings.CompleteBooking;
using MediatR;
using System;

namespace BookingPlatform.Api.Endpoints.Bookings;

public class CompleteBookingEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/bookings/{bookingId:guid}/complete", async (Guid bookingId, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new CompleteBookingCommand(bookingId), ct);
            return result.IsSuccess
                ? Results.Ok()
                : Results.Json(result.Error!.Description, statusCode: result.Error.Code);
        })
        .RequireAuthorization("Owner")
        .WithTags("Bookings")
        .WithSummary("Mark a confirmed booking as completed — owner only");
    }
}
