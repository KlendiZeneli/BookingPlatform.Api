using BookingPlatform.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;
using BookingPlatform.Domain.Enums;
using BookingPlatform.Application.Features.Bookings.CancelBooking;

namespace BookingPlatform.Api.Endpoints.Bookings;

public static class CancelBookingEndpoint
{
    public static void MapCancelBookingEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/bookings/{bookingId:guid}/cancel", async (
            Guid bookingId,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(new CancelBookingCommand(bookingId), ct);
            return result.IsSuccess ? Results.Ok() : Results.Json(result.Error.Description, statusCode: result.Error.Code);
        }).RequireAuthorization("Guest");
    }
}
