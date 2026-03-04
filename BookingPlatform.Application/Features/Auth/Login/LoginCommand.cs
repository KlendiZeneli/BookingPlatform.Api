using System;
using System.Collections.Generic;
using System.Text;

namespace BookingPlatform.Application.Features.Auth.Login;

public record LoginCommand(
       string Email,
         string Password
    );
