using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using BookingPlatform.Application.Common;

namespace BookingPlatform.Application.Features.Auth.Login;

public record LoginCommand : IRequest<Result<LoginResponse>>
{
    public string Email { get; init; }
    public string Password { get; init; }
}
