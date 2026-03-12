using BookingPlatform.Api.Endpoints;
using BookingPlatform.Application.Features.Users.DeleteMe;
using MediatR;

namespace BookingPlatform.Api.Endpoints.Users;

public class DeleteMeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/users/me", async (IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new DeleteMeCommand(), ct);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.Json(result.Error!.Description, statusCode: result.Error.Code);
        })
        .RequireAuthorization()
        .WithTags("Users")
        .WithSummary("Deactivate and delete the current user account");
    }
}
