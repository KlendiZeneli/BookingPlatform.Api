using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BookingPlatform.Domain.Entities;
using BookingPlatform.Application.Interfaces;
using BookingPlatform.Application.Common;

namespace BookingPlatform.Application.Features.Auth.Login;

public class LoginHandler
{
        private readonly IUserRepository _users;
        private readonly IJwtTokenService _tokenService;
    
        public LoginHandler(IUserRepository users, IJwtTokenService tokenService)
        {
            _users = users;
            _tokenService = tokenService;
        }
    
        public async Task<Result<LoginResponse>> Handle(LoginCommand command, CancellationToken ct)
        {
            var user = await _users.GetByEmailAsync(command.Email, ct);
            if (user == null )
            {
            return Errors.UserNotFound;
            }
            if(!BCrypt.Net.BCrypt.Verify(command.Password, user.Password))
        {
            return Errors.InvalidCredentials;
        }
            var token = _tokenService.GenerateToken(user);
            return new LoginResponse {
                Token = token,
            };
    }
}
