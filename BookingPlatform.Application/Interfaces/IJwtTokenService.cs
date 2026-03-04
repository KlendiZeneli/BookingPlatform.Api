using System;
using System.Collections.Generic;
using System.Text;
using BookingPlatform.Domain.Entities;

namespace BookingPlatform.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}
