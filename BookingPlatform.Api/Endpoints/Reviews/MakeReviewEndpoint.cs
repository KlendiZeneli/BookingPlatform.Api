using BookingPlatform.Application.Features.Reviews.MakeReview;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;
using System.Threading;

namespace BookingPlatform.Api.Endpoints.Reviews;

public static class MakeReviewEndpoint
{
    public record MakeReviewRequest(int Rating, string? Comment);

    public static void MapMakeReviewEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/bookings/{bookingId:guid}/review", async (
            System.Guid bookingId,
            MakeReviewRequest req,
            HttpContext http,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var userIdClaim = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !System.Guid.TryParse(userIdClaim, out _))
                return Results.Json("Unauthorized", statusCode: 401);

            var cmd = new MakeReviewCommand(bookingId, req.Rating, req.Comment);
            var result = await mediator.Send(cmd, ct);
            return result.IsSuccess
                ? Results.Created($"/api/reviews/{result.Value.ReviewId}", result.Value)
                : Results.Json(result.Error.Description, statusCode: result.Error.Code);
        }).RequireAuthorization();
    }
}
