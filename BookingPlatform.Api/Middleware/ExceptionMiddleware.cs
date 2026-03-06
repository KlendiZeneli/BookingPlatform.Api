using BookingPlatform.Application.Common;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace BookingPlatform.Api.Middleware;

public static class ExceptionMiddleware
{
    public static void UseGlobalExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(builder =>
        {
            builder.Run(async context =>
            {
                context.Response.ContentType = "application/json";

                var exception = context.Features
                    .Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;

                // Since Error is no longer an Exception, this middleware now only
                // catches truly unexpected exceptions — Result handles domain errors.
                var (statusCode, responseBody) = exception switch
                {
                    BadHttpRequestException => (400, (object)new
                    {
                        code = "ValidationError",
                        message = "Invalid request format."
                    }),
                    ValidationException valEx => (400, (object)new
                    {
                        code = "ValidationError",

                        message = valEx.Message,
                        errors = valEx.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
                    }),
                    _ => (500, (object)new
                    {
                        code = "ServerError",
                        message = "An unexpected error occurred."
                    })
                };

                context.Response.StatusCode = statusCode;
                await context.Response.WriteAsync(JsonSerializer.Serialize(responseBody));
            });
        });
    }
}