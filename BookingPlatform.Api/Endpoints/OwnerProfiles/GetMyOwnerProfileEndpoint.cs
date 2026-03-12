using BookingPlatform.Api.Endpoints;
using BookingPlatform.Application.Features.OwnerProfiles.GetMyOwnerProfile;
using MediatR;

namespace BookingPlatform.Api.Endpoints.OwnerProfiles;

public class GetMyOwnerProfileEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/owner-profiles/me", async (IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetMyOwnerProfileQuery(), ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Json(result.Error!.Description, statusCode: result.Error.Code);
        })
        .RequireAuthorization("Owner")
        .WithTags("OwnerProfiles")
        .WithSummary("Get the current owner's profile and verification status");
    }
}
