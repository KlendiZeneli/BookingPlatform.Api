using BookingPlatform.Application.Features.Bookings.MakeBooking;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;
using System.Threading;

namespace BookingPlatform.Api.Endpoints.Bookings;

public static class MakeBookingEndpoint
{
    public record MakeBookingRequest(DateTime StartDate, DateTime EndDate, int GuestCount);

    public static void MapMakeBookingEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/properties/{propertyId:guid}/bookings", async (
            Guid propertyId,
            MakeBookingRequest req,
            HttpContext http,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var userIdClaim = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var _))
                return Results.Json("Unauthorized", statusCode: 401);

            var cmd = new MakeBookingCommand(propertyId, req.StartDate, req.EndDate, req.GuestCount);
            var result = await mediator.Send(cmd, ct);
            return result.IsSuccess
                ? Results.Created($"/api/bookings/{result.Value.BookingId}", result.Value)
                : Results.Json(result.Error.Description, statusCode: result.Error.Code);
        }).RequireAuthorization("Guest");
    }
}
