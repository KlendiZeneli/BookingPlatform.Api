using BookingPlatform.Application.Features.OwnerProfiles.VerifyOwnerProfile;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Threading;

namespace BookingPlatform.API.Endpoints.OwnerProfiles;

public static class VerifyOwnerProfileEndpoint
{
    internal record VerifyRequest(bool Approve, string? Notes);

    public static void MapVerifyOwnerProfileEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/admin/ownerprofiles/{userId:guid}/verify",
            async (System.Guid userId, VerifyRequest req, IMediator mediator, CancellationToken ct) =>
            {
                var cmd = new VerifyOwnerProfileCommand(userId, req.Approve, req.Notes);
                var result = await mediator.Send(cmd, ct);
                return result.IsSuccess
                    ? Results.Ok()
                    : Results.Json(result.Error.Description, statusCode: result.Error.Code);
            })
            .RequireAuthorization("Guest");
    }
}
