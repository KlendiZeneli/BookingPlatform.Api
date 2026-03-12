using BookingPlatform.Api.Endpoints;
using BookingPlatform.Application.Features.Users.UpdateMe;
using MediatR;

namespace BookingPlatform.Api.Endpoints.Users;

public class UpdateMeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/users/me", async (UpdateMeCommand cmd, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(cmd, ct);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.Json(result.Error!.Description, statusCode: result.Error.Code);
        })
        .RequireAuthorization()
        .WithTags("Users")
        .WithSummary("Update current user's profile");
    }
}
