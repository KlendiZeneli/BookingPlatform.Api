using BookingPlatform.Api.Endpoints;
using BookingPlatform.Application.Features.Bookings.GetMyBookings;
using MediatR;

namespace BookingPlatform.Api.Endpoints.Bookings;

public class GetMyBookingsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/bookings/my", async (IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetMyBookingsQuery(), ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Json(result.Error!.Description, statusCode: result.Error.Code);
        })
        .RequireAuthorization()
        .WithTags("Bookings")
        .WithSummary("Get current guest's bookings");
    }
}
