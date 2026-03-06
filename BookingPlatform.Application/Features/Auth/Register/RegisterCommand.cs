using BookingPlatform.Application.Features.Auth.Login;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using BookingPlatform.Application.Common;

namespace BookingPlatform.Application.Features.Auth.Register;

public record RegisterCommand (
    string FirstName,
    string LastName,
    string Email,
    string Password
) : IRequest<Result<RegisterResponse>>;
