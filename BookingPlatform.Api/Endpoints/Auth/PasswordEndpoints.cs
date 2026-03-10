using BookingPlatform.Application.Features.Auth.RequestPasswordReset;
using BookingPlatform.Application.Features.Auth.ResetPassword;
using BookingPlatform.Application.Features.Auth.ChangePassword;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Threading;

namespace BookingPlatform.API.Endpoints.Auth;

public static class PasswordEndpoints
{
    public static void MapPasswordEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/request-password-reset", async (RequestPasswordResetCommand cmd, IMediator mediator, CancellationToken ct) =>
        {
            var res = await mediator.Send(cmd, ct);
            return res.IsSuccess ? Results.Ok() : Results.Json(res.Error.Description, statusCode: res.Error.Code);
        });

        app.MapPost("/api/auth/reset-password", async (ResetPasswordCommand cmd, IMediator mediator, CancellationToken ct) =>
        {
            var res = await mediator.Send(cmd, ct);
            return res.IsSuccess ? Results.Ok() : Results.Json(res.Error.Description, statusCode: res.Error.Code);
        });

        app.MapPost("/api/auth/change-password", async (ChangePasswordCommand cmd, IMediator mediator, CancellationToken ct) =>
        {
            var res = await mediator.Send(cmd, ct);
            return res.IsSuccess ? Results.Ok() : Results.Json(res.Error.Description, statusCode: res.Error.Code);
        }).RequireAuthorization();
    }
}
