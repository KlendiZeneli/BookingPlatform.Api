using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BookingPlatform.Domain.Entities;
using BookingPlatform.Application.Interfaces;

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
    
        public async Task<LoginResponse> Handle(LoginCommand command, CancellationToken ct)
        {
            var user = await _users.GetByEmailAsync(command.Email, ct);
            if (user == null || !BCrypt.Net.BCrypt.Verify(command.Password, user.Password))
            {
                throw new Exception("Invalid email or password.");
            }
            var token = _tokenService.GenerateToken(user);
            return new LoginResponse {
                Token = token,
            };
    }
}
