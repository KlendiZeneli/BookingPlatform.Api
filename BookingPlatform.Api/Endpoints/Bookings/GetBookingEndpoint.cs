using BookingPlatform.Api.Endpoints;
using BookingPlatform.Application.Features.Bookings.GetBooking;
using MediatR;

namespace BookingPlatform.Api.Endpoints.Bookings;

public class GetBookingEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/bookings/{bookingId:guid}", async (Guid bookingId, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetBookingQuery(bookingId), ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Json(result.Error!.Description, statusCode: result.Error.Code);
        })
        .RequireAuthorization()
        .WithTags("Bookings")
        .WithSummary("Get a single booking by id");
    }
}
