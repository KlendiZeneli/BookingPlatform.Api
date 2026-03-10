using BookingPlatform.Api.Endpoints;
using BookingPlatform.Application.Features.Properties.SearchProperties;
using MediatR;

namespace BookingPlatform.API.Endpoints.Properties;

public class SearchPropertiesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/properties", async (
            [AsParameters] SearchPropertiesCommand command,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(command, ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Json(new { code = result.Error!.Id, message = result.Error.Description },
                    statusCode: result.Error.Code);
        })
        .WithTags("Properties")
        .WithSummary("Search and filter properties")
        .AllowAnonymous()
        .Produces<PagedResponse<PropertyResponse>>()
        .ProducesValidationProblem();
    }
}