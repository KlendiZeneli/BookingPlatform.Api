using BookingPlatform.Api.Endpoints;
using BookingPlatform.Application.Features.Users.GetMe;
using MediatR;

namespace BookingPlatform.Api.Endpoints.Users;

public class GetMeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users/me", async (IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetMeQuery(), ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Json(result.Error!.Description, statusCode: result.Error.Code);
        })
        .RequireAuthorization()
        .WithTags("Users")
        .WithSummary("Get current authenticated user profile");
    }
}
