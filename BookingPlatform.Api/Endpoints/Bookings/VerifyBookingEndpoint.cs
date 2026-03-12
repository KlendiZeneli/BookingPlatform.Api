using BookingPlatform.Application.Features.Bookings.VerifyBooking;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;using System.Security.Claims;
using System.Threading;

namespace BookingPlatform.Api.Endpoints.Bookings;

public static class VerifyBookingEndpoint
{
    public static void MapVerifyBookingEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/bookings/{bookingId:guid}/verify", async (
            Guid bookingId,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var cmd = new VerifyBookingCommand(bookingId);
            var result = await mediator.Send(cmd, ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Json(result.Error.Description, statusCode: result.Error.Code);
        })
        .RequireAuthorization("Owner")
        .WithTags("Bookings")
        .WithSummary("Confirm a pending booking — owner only");
    }
}
