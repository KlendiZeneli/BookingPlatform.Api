using BookingPlatform.Application.Common;
using BookingPlatform.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BookingPlatform.Application.Features.Auth.Login;

public class LoginHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly IUserRepository _users;
    private readonly IJwtTokenService _tokenService;
    private readonly ILogger<LoginHandler> _logger;

    public LoginHandler(IUserRepository users, IJwtTokenService tokenService, ILogger<LoginHandler> logger)
    {
        _users = users;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand command, CancellationToken ct)
    {
        var user = await _users.GetByEmailAsync(command.Email, ct);

        if (user == null)
            return Errors.UserNotFound;

        if (!BCrypt.Net.BCrypt.Verify(command.Password, user.Password))
            return Errors.InvalidCredentials;

        var token = _tokenService.GenerateToken(user);

        return new LoginResponse { Token = token };
    }
}