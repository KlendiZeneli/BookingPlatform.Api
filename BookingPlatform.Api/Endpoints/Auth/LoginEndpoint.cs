using BookingPlatform.Application.Common;
using BookingPlatform.Application.Features.Auth.Login;
using BookingPlatform.Application.Features.Auth.Register;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Threading;
using MediatR;

namespace BookingPlatform.API.Endpoints.Auth;

public static class LoginEndpoint
{
    public static void MapLoginEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login",
            async (LoginCommand command, IMediator mediator, CancellationToken ct) =>
            {

                var result = await mediator.Send(command, ct);


                return result.IsSuccess
                   ? Results.Ok(result.Value)
                   : Results.Json(result.Error.Description, statusCode: result.Error.Code);
            });
    }   
}
