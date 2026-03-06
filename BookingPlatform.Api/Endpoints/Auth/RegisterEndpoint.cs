using BookingPlatform.Application.Common;
using BookingPlatform.Application.Features.Auth.Register;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Threading;

namespace BookingPlatform.API.Endpoints.Auth;

public static class RegisterEndpoint
{
    public static void MapRegisterEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/register",
            async (RegisterCommand command, IMediator mediator, CancellationToken ct) =>
            {
                var result = await mediator.Send(command, ct);

                return result.IsSuccess
                   ? Results.Ok(result.Value)
                   : Results.Json(result.Error.Description, statusCode: result.Error.Code);
            });
    }
}
