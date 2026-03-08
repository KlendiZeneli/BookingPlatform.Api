using BookingPlatform.Application.Features.OwnerProfiles.CreateOwnerProfile;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;
using System.Threading;

namespace BookingPlatform.API.Endpoints.OwnerProfiles;

public static class CreateOwnerProfileEndpoint
{
    internal record CreateRequest(string IdentityCardNumber, string? BusinessName, string CreditCard);

    public static void MapCreateOwnerProfileEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/create-owner-profile",
            async (CreateRequest req, HttpContext http, IMediator mediator, CancellationToken ct) =>
            {
                var userIdClaim = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !System.Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.Json("Hello", statusCode: 401);
                }

                var cmd = new CreateOwnerProfileCommand(userId, req.IdentityCardNumber, req.BusinessName, req.CreditCard);
                var result = await mediator.Send(cmd, ct);
                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.Json(result.Error.Description, statusCode: result.Error.Code);
            })
            .RequireAuthorization("Guest");
    }
}
